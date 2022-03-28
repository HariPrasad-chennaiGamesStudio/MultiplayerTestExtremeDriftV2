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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class RabbitMQController : MonoBehaviour
{
    public static RabbitMQController instance;

    //Currently using '.Net Standard 2.0' dlls.  
    //Based on Project Settings .Net compatability, Need to use the dlls accordingly
    
    //Durable(the queue will survive a broker restart)
    //Exclusive(used by only one connection and the queue will be deleted when that connection closes)
    //Auto-delete(queue that has had at least one consumer is deleted when last consumer unsubscribes)
    //Arguments(optional; used by plugins and broker-specific features such as message TTL, queue length limit, etc)

    private EventingBasicConsumer consumer;
    private IConnection connection;
    private IModel sendChannel;
    private IModel receiveChannel;

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
        Debug.Log("queueNameSend = " + client_Id);
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
            sendChannel = connection.CreateModel();
            receiveChannel = connection.CreateModel();

            Debug.Log("CONNECTED");
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.ToString());
            Debug.Log("Stack Trace: " + e.StackTrace.ToString());
            Debug.Log("Message: " + e.Message);
            Debug.Log("Inner Exception: " + e.InnerException.ToString());

            Disconnect();
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

            receiveChannel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: false, autoDelete: true, arguments: null);

            //var args = new Dictionary<string, object>();
            //args.Add("x-message-ttl", 500);
            var queueName = receiveChannel.QueueDeclare(durable:false, autoDelete: true, arguments: null).QueueName;


        receiveChannel.QueueBind(queue: queueName,
                                  exchange: exchangeName,
                                  routingKey: "",
                                  arguments: null);

            consumer = new EventingBasicConsumer(receiveChannel);

            consumer.Received += OnReceiveMessageFromQueue;
            receiveChannel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
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
            //var args = new Dictionary<string, object>();
            //args.Add("x-message-ttl", 5000);

            sendChannel.QueueDeclare(queue: queueNameSend,
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
        if (sendChannel != null)
        {
            //var body = Encoding.UTF8.GetBytes(message);
            //Debug.Log(queueNameSend + " -- SendMessage == " + message);

            //channel.BasicPublish(exchange: "",
            //                     routingKey: queueNameSend,//queue name
            //                     basicProperties: null,
            //                     body: body);

            Debug.Log("Send Message == " + message);

            byte[] body = Encoding.UTF8.GetBytes(message);

            IBasicProperties props = sendChannel.CreateBasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = 2;
            props.Persistent = false;
            props.Expiration = "10000";

            sendChannel.BasicPublish(exchange: "",
                               routingKey: queueNameSend,
                               basicProperties: props,
                               body: body);

            body = null;
        }
    }

    public void SendMessageInQueue(object message)
    {
        if (sendChannel != null)
        {
            //var body = Encoding.UTF8.GetBytes(message);
            //Debug.Log(queueNameSend + " -- SendMessage == " + message);

            //channel.BasicPublish(exchange: "",
            //                     routingKey: queueNameSend,//queue name
            //                     basicProperties: null,
            //                     body: body);

            Debug.Log("Send Message == " + BitConverter.ToString(ObjectToByteArray(message)));

            //byte[] body = Encoding.UTF8.GetBytes(message);

            IBasicProperties props = sendChannel.CreateBasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = 2;
            props.Persistent = false;
            props.Expiration = "10000";

            sendChannel.BasicPublish(exchange: "",
                               routingKey: queueNameSend,
                               basicProperties: props,
                               body: ObjectToByteArray(message));
        }
    }

    private void OnReceiveMessageFromQueue(object sender, BasicDeliverEventArgs ea)
    {
        //var body = ea.Body.ToArray();
        //var message = Encoding.UTF8.GetString(body);

        _executionQueue.Enqueue(() => {
            TriggetOnMessageReceived(ea.Body.ToArray());
        });

        //channel.BasicAck(ea.DeliveryTag, false);
    }

    [System.Serializable]
    private struct ReceivedData
    {
        public int action;
        public string id;
        public MultiplayerPlayersPlacement data;
    }

    private ReceivedData _receivedData;

    private void TriggetOnMessageReceived(byte[] _data)
    {
        //Debug.Log("Consumed Message TOP");
        //Debug.Log("Consumed Message TOP == " + BitConverter.ToString(_data));
        //_receivedData = ByteArrayToObject<ReceivedData>(_data);
        //Debug.Log(_receivedData.action + " :: " + _receivedData.id);
        //if (_receivedData.action == 3)
        //{
        //    NetworkClient.instance.UpdatePosition(_receivedData.id, _receivedData.data);
        //}

        //var _rData = ByteArrayToObject<MultiplayerPlayersPlacement>(_data);
        //NetworkClient.instance.UpdatePosition(_rData.id, _rData);

        //Debug.Log(_rData.id + " == " + _rData.p.x + " :: " + _rData.p.y + " :: " + _rData.p.z);


        //Debug.Log(" == " + Encoding.UTF8.GetString(_data));
        NetworkClient.instance.UpdatePosition(Encoding.UTF8.GetString(_data).Split('|'));

        _data = null;

        //JObject jobj = JObject.Parse(message);

        //int action = (int)jobj["action"];

        //if (action == 3)
        //{
        //    //NetworkClient.instance.UpdatePosition((string)jobj["id"], (float)jobj["x"], (float)jobj["y"], (float)jobj["z"]);
        //    NetworkClient.instance.UpdatePosition((string)jobj["id"], jobj["data"].ToString());
        //}
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

        if (receiveChannel != null)
        {
            receiveChannel.Close();
            receiveChannel.Dispose();
            receiveChannel = null;
        }

        if (sendChannel != null)
        {
            sendChannel.Close();
            sendChannel.Dispose();
            sendChannel = null;
        }

        if (connection != null)
        {
            connection.Close();
            connection.Dispose();
            connection = null;
        }
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

    public byte[] ObjectToByteArray(object obj)
    {
        using (var ms = new MemoryStream())
        {
            var bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    public T ByteArrayToObject<T>(byte[] arrBytes)
    {
        using (var ms = new MemoryStream(arrBytes))
        {
            var _BinaryFormatter = new BinaryFormatter();
            return (T)_BinaryFormatter.Deserialize(ms);
        }
    }
}
