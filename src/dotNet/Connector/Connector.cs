/*
 * Copyright 2017 Red Hat Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using ClientLib;

namespace Connector
{
    /// <summary>
    /// Class represent receiver from amqp broker
    /// </summary>
    class Aac2Connector
    {
        /// <summary>
        /// Main method of receiver
        /// </summary>
        /// <param name="args">args from command line</param>
        static void Main(string[] args)
        {
            ConnectorClient client = new ConnectorClient();
            client.Run(args);
        }
    }
}