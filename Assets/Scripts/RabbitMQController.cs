using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using UnityEngine.Events;
using RabbitMQ.Client.Logging;

public class RabbitMQController : MonoBehaviour
{
    public static RabbitMQController instance;
    private EventingBasicConsumer consumer;
    private IConnection connection;
    private IModel channel;

    private Task connectionTask;

    private static readonly Queue<UnityAction> _executionQueue = new Queue<UnityAction>();

    ////Take Build
    //private string _receiveQueneName = "queue1";
    //private string _sendQueneName = "queue2";
    //public bool sendData = false;

    ////Editor
    //private string _receiveQueneName = "queue3";
    //private string _sendQueneName = "queue1";
    //public bool sendData = true;


    //private string _receiveQueneName = "queue1";
    //private string _sendQueneName = "queue2";
    //public bool sendData;

    private string exchangeName = "roomid";
    private string queueNameSend;



    void Start()
    {
        connectionTask = Task.Run(Connect);

        instance = this;
    }

    public void UpdateQueueName(string client_Id)
    {
        queueNameSend = client_Id;
    }

    public void UpdateExchangeKeyAndCreateQueue(string _exchangeName)
    {
        Debug.Log("exchangeName == " + exchangeName);
        exchangeName = _exchangeName;

        CreateSendQueue();
        CreateReceiveQueue();
        Debug.Log("--------");
    }

    private void Connect()
    {
        try
        {
            var factory = new ConnectionFactory();
            //factory.Uri = new Uri("amqp://skumar:12345@192.168.14.138:5672");
            factory.Uri = new Uri("amqp://admin:admin@3.110.7.51:5672");

            //var endpoints = new System.Collections.Generic.List<AmqpTcpEndpoint> {
            //  new AmqpTcpEndpoint("hostname"),
            //  new AmqpTcpEndpoint("localhost")
            //};
            //connection = factory.CreateConnection(endpoints);


            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            Debug.Log("CONNECTED");
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.ToString());
            Debug.Log("Stack Trace: " + e.StackTrace.ToString());
            Debug.Log("Message: " + e.Message);
            Debug.Log("Inner Exception: " + e.InnerException.ToString());

            if (channel != null)
            {
                channel.Close();
                channel.Dispose();
            }

            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }
        }
    }

    private void CreateReceiveQueue()
    {
        //try
        //{
            //channel.QueueDeclare(queue: _receiveQueneName,
            //                                durable: false,
            //                                exclusive: false,
            //                                autoDelete: true,
            //                                arguments: null);

            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: false, autoDelete: true, arguments: null);

            var args = new Dictionary<string, object>();
            args.Add("x-message-ttl", 500);
            var queueName = channel.QueueDeclare(durable:false, autoDelete: true, exclusive:false, arguments: args).QueueName;

        
            channel.QueueBind(queue: queueName,
                                  exchange: exchangeName,
                                  routingKey: "",
                                  arguments: args);

            consumer = new EventingBasicConsumer(channel);

            consumer.Received += OnReceiveMessageFromQueue;
            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        //}
        //catch (Exception e)
        //{
        //    Debug.Log("Exception: " + e.ToString());
        //    Debug.Log("Stack Trace: " + e.StackTrace.ToString());
        //    Debug.Log("Message: " + e.Message);
        //    Debug.Log("Inner Exception: " + e.InnerException.ToString());

        //    //if (channel != null)
        //    //{
        //    //    channel.Close();
        //    //    channel.Dispose();
        //    //}

        //    //if (connection != null)
        //    //{
        //    //    connection.Close();
        //    //    connection.Dispose();
        //    //}
        //}
    }

    private void CreateSendQueue()
    {
        Debug.Log("queueNameSend == " + queueNameSend);

        //try
        //{ 
            var args = new Dictionary<string, object>();
            args.Add("x-message-ttl", 5000);

            channel.QueueDeclare(queue: queueNameSend,
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: true,
                                            arguments: null);

            //channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);
        //}
        //catch (Exception e)
        //{
        //    Debug.Log("Exception: " + e.ToString());
        //    Debug.Log("Stack Trace: " + e.StackTrace.ToString());
        //    Debug.Log("Message: " + e.Message);
        //    Debug.Log("Inner Exception: " + e.InnerException.ToString());

        //    //if (channel != null)
        //    //{
        //    //    channel.Close();
        //    //    channel.Dispose();
        //    //}

        //    //if (connection != null)
        //    //{
        //    //    connection.Close();
        //    //    connection.Dispose();
        //    //}
        //}
    }


    public void SendMessageInQueue(string message)
    {
        if (channel != null)
        {
            //var body = Encoding.UTF8.GetBytes(message);
            //Debug.Log(queueNameSend + " -- SendMessage == " + message);

            //channel.BasicPublish(exchange: "",
            //                     routingKey: queueNameSend,//queue name
            //                     basicProperties: null,
            //                     body: body);

            //Debug.Log("Send Message == " + message);

            byte[] body = Encoding.UTF8.GetBytes(message);

            IBasicProperties props = channel.CreateBasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = 2;
            props.Persistent = false;
            props.Expiration = "5000";

            channel.BasicPublish(exchange: "",
                               routingKey: queueNameSend,
                               basicProperties: props,
                               body: body);
        }
    }

    private void OnReceiveMessageFromQueue(object sender, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        _executionQueue.Enqueue(() => {
            TriggetOnMessageReceived(message);
        });

        //channel.BasicAck(ea.DeliveryTag, false);
    }

    private void TriggetOnMessageReceived(string message)
    {
        //Debug.Log("Consumed Message = " + message);

        JObject jobj = JObject.Parse(message);

        int action = (int)jobj["action"];

        if (action == 3)
        {
            //NetworkClient.instance.UpdatePosition((string)jobj["id"], (float)jobj["x"], (float)jobj["y"], (float)jobj["z"]);
            NetworkClient.instance.UpdatePosition((string)jobj["id"], jobj["data"].ToString());
        }
        //else if (action == 1)
        //{
        //    string innerMessage = (string)jobj["color"];
        //    Controller.instance.UpdateColor(innerMessage);
        //}
        //else if (action == 2)
        //{
        //    Controller.instance.UpdateValue((float)jobj["valueX"], (float)jobj["valueZ"]);
        //}
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    private void Disconnect()
    {
        //connectionTask.Dispose();
        if (consumer != null)
        {
            consumer.Received -= OnReceiveMessageFromQueue;
            consumer = null;
        }
        channel.Close();
        channel.Dispose();
        channel = null;
        connection.Close();
        connection.Dispose();
        connection = null;

        _executionQueue.Clear();
    }

    private void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }
}
