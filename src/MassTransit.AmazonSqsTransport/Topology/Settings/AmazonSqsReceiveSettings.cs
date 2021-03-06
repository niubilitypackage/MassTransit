// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.AmazonSqsTransport.Topology.Settings
{
    using System;
    using Configuration.Configurators;


    public class AmazonSqsReceiveSettings :
        QueueBindingConfigurator,
        ReceiveSettings
    {
        public AmazonSqsReceiveSettings(string queueName, bool durable, bool autoDelete)
            : base(queueName, durable, autoDelete)
        {
            PrefetchCount = Math.Min(Environment.ProcessorCount * 2, 10);
            WaitTimeSeconds = 0;
        }

        public int PrefetchCount { get; set; }

        public int WaitTimeSeconds { get; set; }

        public bool PurgeOnStartup { get; set; }

        public Uri GetInputAddress(Uri hostAddress)
        {
            var builder = new UriBuilder(hostAddress);

            builder.Path = builder.Path == "/"
                ? $"/{EntityName}"
                : $"/{string.Join("/", builder.Path.Trim('/'), EntityName)}";

            builder.Query += string.Join("&", GetQueryStringOptions());

            return builder.Uri;
        }
    }
}
