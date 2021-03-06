﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Diagnostics.EventFlow.HealthReporters;
using Microsoft.Diagnostics.EventFlow.Inputs;
using Microsoft.Diagnostics.EventFlow.Outputs;

namespace Microsoft.Diagnostics.EventFlow.Consumers.ConsoleAppCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // HealthReporter
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("config.json");
            var configuration = configBuilder.Build();

            using (IHealthReporter reporter = new CsvHealthReporter("FileReportConfig.json"))
            {
                // Listeners
                List<IObservable<EventData>> inputs = new List<IObservable<EventData>>();
                inputs.Add((new TraceInputFactory()).CreateItem(configuration, reporter));

                // Senders
                var outputs = new List<IOutput>();
                outputs.Add(new StdOutput(reporter));

                DiagnosticPipeline pipeline = new DiagnosticPipeline(
                    reporter, 
                    inputs,
                    null,
                    new EventSink[] { new EventSink(new StdOutput(reporter), null) });

                // Build up the pipeline
                Console.WriteLine("Pipeline is created.");

                // Send a trace to the pipeline
                Trace.TraceInformation("This is a message from trace . . .");
                Trace.TraceWarning("This is a warning from trace . . .");


                // Check the result
                Console.WriteLine("Press any key to continue . . .");
                Console.ReadKey(true);
            }
        }
    }
}
