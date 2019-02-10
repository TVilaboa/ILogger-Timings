// Copyright 2016 ILoggerTimings Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;


namespace ILoggerTimings.Configuration
{
    /// <summary>
    /// Launches <see cref="Operation"/>s with non-default completion and abandonment levels.
    /// </summary>
    /// <seealso cref="Operation.At"/>
    public class LevelledOperation
    {
        readonly Operation _cachedResult;

        readonly ILogger _logger;
        readonly LogLevel _completion;
        readonly LogLevel _abandonment;

        internal LevelledOperation(ILogger logger, LogLevel completion, LogLevel abandonment)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _logger = logger;
            _completion = completion;
            _abandonment = abandonment;
        }

        LevelledOperation(Operation cachedResult)
        {
            if (cachedResult == null) throw new ArgumentNullException(nameof(cachedResult));
            _cachedResult = cachedResult;
        }

        private static LevelledOperation None(ILogger _logger)
        {
            return new LevelledOperation(
                new Operation(_logger,
                    "", new Dictionary<string, object>(), 
                    CompletionBehaviour.Silent,
                    LogLevel.Critical,
                    LogLevel.Critical));
        } 

        internal static LevelledOperation GetNone(ILogger _logger)
        {
            return None(_logger);
        }

        /// <summary>
        /// Begin a new timed operation. The return value must be completed using <see cref="Operation.Complete()"/>,
        /// or disposed to record abandonment.
        /// </summary>
        /// <param name="messageTemplate">A log message describing the operation, in message template format.</param>
        /// <param name="args">Arguments to the log message. These will be stored and captured only when the
        /// operation completes, so do not pass arguments that are mutated during the operation.</param>
        /// <returns>An <see cref="Operation"/> object.</returns>
        public Operation Begin(string messageTemplate, IDictionary<string, object> args)
        {
            return _cachedResult ?? new Operation(_logger, messageTemplate, args, CompletionBehaviour.Abandon, _completion, _abandonment);
        }

        /// <summary>
        /// Begin a new timed operation. The return value must be disposed to complete the operation.
        /// </summary>
        /// <param name="messageTemplate">A log message describing the operation, in message template format.</param>
        /// <param name="args">Arguments to the log message. These will be stored and captured only when the
        /// operation completes, so do not pass arguments that are mutated during the operation.</param>
        /// <returns>An <see cref="Operation"/> object.</returns>
        public IDisposable Time(string messageTemplate, IDictionary<string, object> args)
        {
            return _cachedResult ?? new Operation(_logger, messageTemplate, args, CompletionBehaviour.Complete, _completion, _abandonment);
        }
    }
}