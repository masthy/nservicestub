using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NServiceStub.WCF;
using NUnit.Framework;

namespace NServiceStub.IntegrationTests.WCF
{
    [TestFixture]
    public class WcfTriggeredMessageSequenceTests
    {
        [Test]
        public void ExecuteNextStep_ExecutingWhileTriggeringNewSequenceOfEventsAsync_CanHandleAsyncExecuteAndAddNewSequences()
        {
            // Arrange
            var sequence = new WcfTriggeredMessageSequence();

            var executionContext = new SequenceExecutionContext(new List<IMessageSequence> {sequence}, "whatever", new Mock<IMessagePicker>().Object);

            sequence.SetNextStep(new SendMessage(new Mock<ISender>().Object));

            // Act
            IList<Task> tasks = new List<Task>();

            for (int i = 0; i < 1000; i++)
            {
                var execute = new Task(obj => sequence.ExecuteNextStep(obj as SequenceExecutionContext), executionContext);
                var triggerNewSequenceOfEvents = new Task(sequence.TriggerNewSequenceOfEvents);

                tasks.Add(execute);
                tasks.Add(triggerNewSequenceOfEvents);

                execute.Start();
                triggerNewSequenceOfEvents.Start();
            }

            foreach (var task in tasks)
            {
                task.Wait();
            }

            // Assert
            Assert.That(tasks.All(task => task.IsCompleted));

        } 
    }
}