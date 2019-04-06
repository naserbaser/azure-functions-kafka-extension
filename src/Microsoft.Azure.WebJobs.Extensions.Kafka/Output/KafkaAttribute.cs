﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Avro.Specific;
using Microsoft.Azure.WebJobs.Description;

namespace Microsoft.Azure.WebJobs.Extensions.Kafka
{
    /// <summary>
    /// Setup an 'output' binding to an Kafka topic. This can be any output type compatible with an IAsyncCollector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public sealed class KafkaAttribute : Attribute
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="KafkaAttribute"/>
        /// </summary>
        /// <param name="brokerList">Broker list</param>
        /// <param name="topic">Topic name</param>
        public KafkaAttribute(string brokerList, string topic)
        {
            BrokerList = brokerList;
            Topic = topic;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.WebJobs.Extensions.Kafka.KafkaAttribute"/> class.
        /// </summary>
        public KafkaAttribute()
        {
        }

        /// <summary>
        /// The topic name hub.
        /// </summary>
        [AutoResolve]
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the Broker List.
        /// </summary>
        // [ConnectionString]
        public string BrokerList { get; set; }


        /// <summary>
        /// Gets or sets the key element type
        /// Default is long
        /// </summary>
        public Type KeyType { get; set; }

        Type valueType;

        /// <summary>
        /// Gets or sets the Avro data type
        /// Must implement ISpecificRecord
        /// </summary>
        public Type ValueType
        {
            get => this.valueType;
            set
            {
                if (value != null && !IsValidValueType(value))
                {
                    throw new ArgumentException($"The value of {nameof(ValueType)} must be a byte[], string or a type that implements {nameof(ISpecificRecord)} or {nameof(Google.Protobuf.IMessage)}. The type {value.Name} does not.");
                }

                this.valueType = value;
            }
        }

        /// <summary>
        /// Gets or sets the Avro schema.
        /// Should be used only if a generic record should be generated
        /// </summary>
        public string AvroSchema { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the topic should be created if it does not exist
        /// Default: false
        /// </summary>
        public bool CreateTopicIfNotExists { get; set; }

        /// <summary>
        /// Gets or sets the topic partition count, in case <see cref="CreateTopicIfNotExists"/> is true
        /// Default: 1
        /// </summary>
        public int TopicPartitionCount { get; set; } = 1;

        /// <summary>
        /// Gets or sets the topic replication factor, in case <see cref="CreateTopicIfNotExists"/> is true
        /// Default: 1
        /// </summary>
        public short TopicReplicationFactor { get; set; } = 1;


        /// <summary>
        /// Validates if the <see cref="ValueType"/> is valid
        /// </summary>
        bool IsValidValueType(Type value)
        {
            return
                typeof(ISpecificRecord).IsAssignableFrom(value) ||
                typeof(Google.Protobuf.IMessage).IsAssignableFrom(value) ||
                value == typeof(byte[]) ||
                value == typeof(string);
        }
    }
}