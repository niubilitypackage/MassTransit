﻿// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Builders
{
	using System;
	using Exceptions;
	using Magnum;
	using Transports;
	using Util;

	public class EndpointBuilderImpl :
		EndpointBuilder
	{
		readonly ITransportSettings _errorSettings;
		readonly Func<ITransportFactory, ITransportSettings, IOutboundTransport> _errorTransportFactory;
		readonly IEndpointSettings _settings;
		readonly Func<ITransportFactory, ITransportSettings, IDuplexTransport> _transportFactory;
		readonly Uri _uri;

		public EndpointBuilderImpl([NotNull] Uri uri, [NotNull] IEndpointSettings settings,
		                           [NotNull] ITransportSettings errorSettings,
		                           [NotNull] Func<ITransportFactory, ITransportSettings, IDuplexTransport> transportFactory,
		                           [NotNull] Func<ITransportFactory, ITransportSettings, IOutboundTransport>
		                           	errorTransportFactory)
		{
			Guard.AgainstNull(uri, "uri");

			_uri = uri;
			_settings = settings;
			_errorSettings = errorSettings;
			_transportFactory = transportFactory;
			_errorTransportFactory = errorTransportFactory;
		}

		public IEndpoint CreateEndpoint(ITransportFactory transportFactory)
		{
			try
			{
				IDuplexTransport transport = _transportFactory(transportFactory, _settings);
				IOutboundTransport errorTransport = _errorTransportFactory(transportFactory, _errorSettings);

				var endpoint = new Endpoint(transport.Address, _settings.Serializer, transport, errorTransport);

				return endpoint;
			}
			catch (Exception ex)
			{
				throw new EndpointException(_uri, "Failed to create endpoint", ex);
			}
		}
	}
}