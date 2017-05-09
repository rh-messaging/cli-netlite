//  ------------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation
//  All rights reserved. 
//  
//  Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this 
//  file except in compliance with the License. You may obtain a copy of the License at 
//  http://www.apache.org/licenses/LICENSE-2.0  
//  
//  THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
//  EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR 
//  CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR 
//  NON-INFRINGEMENT. 
// 
//  See the Apache Version 2.0 License for specific language governing permissions and 
//  limitations under the License.
//  ------------------------------------------------------------------------------------

using Dotnetlite;

namespace Receiver
{ 
    /// <summary>
    /// Class represent receiver from amqp broker
    /// </summary>
    class Receiver
    {
        //
        // Sample invocation: Receiver.exe --broker localhost:5672 --timeout 30 --address my-queue
        //

        /// <summary>
        /// Main method of receiver
        /// </summary>
        /// <param name="args">args from command line</param>
        static void Main(string[] args)
        {
            ReceiverClient client = new ReceiverClient();
            client.Run(args);
        }
    }
}
