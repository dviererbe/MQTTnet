﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Server;

namespace MQTTnet.Benchmarks
{
    [SimpleJob(RuntimeMoniker.Net461)]
    [RPlotExporter, RankColumn]
    [MemoryDiagnoser]
    public class MessageProcessingBenchmark
    {
        IMqttServer _mqttServer;
        IMqttClient _mqttClient;
        MqttApplicationMessage _message;

        [GlobalSetup]
        public void Setup()
        {
            var factory = new MqttFactory();
            _mqttServer = factory.CreateMqttServer();
            _mqttClient = factory.CreateMqttClient();

            var serverOptions = new MqttServerOptionsBuilder().Build();
            _mqttServer.StartAsync(serverOptions).GetAwaiter().GetResult();

            var clientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost").Build();

            _mqttClient.ConnectAsync(clientOptions).GetAwaiter().GetResult();

            _message = new MqttApplicationMessageBuilder()
                .WithTopic("A")
                .Build();
        }

        [Benchmark]
        public void Send_10000_Messages()
        {
            for (var i = 0; i < 10000; i++)
            {
                _mqttClient.PublishAsync(_message).GetAwaiter().GetResult();
            }
        }
    }
}
