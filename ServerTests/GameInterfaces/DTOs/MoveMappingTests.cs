using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle2.Data; // Assuming this is the correct namespace for Move, Trigger, Outcomes, Outcome, Source
using Server.GameInterfaces.DTOs; // For MoveDTO, MoveOutcomeDTO, etc. and MoveExtensions
using System.Collections.Generic;
using System.Linq;

// Minimal mock classes if not fully defined in TheOracle2.Data for testing purposes
// These would ideally be in TheOracle2.Data, but if not, define them here for the test's assumptions.
// For this task, we assume they are defined in TheOracle2.Data as per MoveExtensions.cs's using statements.
/*
namespace TheOracle2.Data
{
    public class Outcome { public string Text { get; set; } }
    public class Outcomes 
    {
        public Outcome StrongHit { get; set; }
        public Outcome WeakHit { get; set; }
        public Outcome Miss { get; set; }
    }
    public class Trigger { public string Text { get; set; } }
    // Assuming Source is also in TheOracle2.Data with a Name property
    // public class Source { public string Name { get; set; } } 
}
*/

namespace ServerTests.GameInterfaces.DTOs
{
    [TestClass]
    public class MoveMappingTests
    {
        [TestMethod]
        public void TestMoveOutcomeToMoveOutcomeDTOMapping_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var outcome = new Outcome { Text = "A success!" };

            // Act
            var outcomeDto = outcome.ToDTO();

            // Assert
            Assert.IsNotNull(outcomeDto);
            Assert.AreEqual(outcome.Text, outcomeDto.Text);
        }

        [TestMethod]
        public void TestMoveOutcomeToMoveOutcomeDTOMapping_NullSource_ShouldReturnNull()
        {
            // Arrange
            Outcome outcome = null;

            // Act
            var outcomeDto = outcome.ToDTO();

            // Assert
            Assert.IsNull(outcomeDto);
        }

        [TestMethod]
        public void TestMoveOutcomesToMoveOutcomesDTOMapping_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var outcomes = new Outcomes
            {
                StrongHit = new Outcome { Text = "Strong hit text" },
                WeakHit = new Outcome { Text = "Weak hit text" },
                Miss = new Outcome { Text = "Miss text" }
            };

            // Act
            var outcomesDto = outcomes.ToDTO();

            // Assert
            Assert.IsNotNull(outcomesDto);
            Assert.IsNotNull(outcomesDto.StrongHit);
            Assert.AreEqual(outcomes.StrongHit.Text, outcomesDto.StrongHit.Text);
            Assert.IsNotNull(outcomesDto.WeakHit);
            Assert.AreEqual(outcomes.WeakHit.Text, outcomesDto.WeakHit.Text);
            Assert.IsNotNull(outcomesDto.Miss);
            Assert.AreEqual(outcomes.Miss.Text, outcomesDto.Miss.Text);
        }

        [TestMethod]
        public void TestMoveOutcomesToMoveOutcomesDTOMapping_NullSource_ShouldReturnNull()
        {
            // Arrange
            Outcomes outcomes = null;

            // Act
            var outcomesDto = outcomes.ToDTO();

            // Assert
            Assert.IsNull(outcomesDto);
        }

        [TestMethod]
        public void TestMoveOutcomesToMoveOutcomesDTOMapping_NullIndividualOutcomes_ShouldMapToNullDTOs()
        {
            // Arrange
            var outcomes = new Outcomes
            {
                StrongHit = null,
                WeakHit = new Outcome { Text = "Weak hit text" },
                Miss = null
            };

            // Act
            var outcomesDto = outcomes.ToDTO();

            // Assert
            Assert.IsNotNull(outcomesDto);
            Assert.IsNull(outcomesDto.StrongHit);
            Assert.IsNotNull(outcomesDto.WeakHit);
            Assert.AreEqual(outcomes.WeakHit.Text, outcomesDto.WeakHit.Text);
            Assert.IsNull(outcomesDto.Miss);
        }

        [TestMethod]
        public void TestMoveTriggerToMoveTriggerDTOMapping_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var trigger = new Trigger { Text = "When you make a move..." };

            // Act
            var triggerDto = trigger.ToDTO();

            // Assert
            Assert.IsNotNull(triggerDto);
            Assert.AreEqual(trigger.Text, triggerDto.Text);
        }

        [TestMethod]
        public void TestMoveTriggerToMoveTriggerDTOMapping_NullSource_ShouldReturnNull()
        {
            // Arrange
            Trigger trigger = null;

            // Act
            var triggerDto = trigger.ToDTO();

            // Assert
            Assert.IsNull(triggerDto);
        }

        [TestMethod]
        public void TestMoveToMoveDTOMapping_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var move = new Move
            {
                Id = "test_move_id",
                Name = "Test Move",
                Category = "Test Category",
                Text = "This is the move text.",
                Optional = true,
                ProgressMove = false,
                VariantOf = "original_move_id",
                Trigger = new Trigger { Text = "When something happens..." },
                Outcomes = new Outcomes
                {
                    StrongHit = new Outcome { Text = "Strong hit outcome" },
                    WeakHit = new Outcome { Text = "Weak hit outcome" },
                    Miss = new Outcome { Text = "Miss outcome" }
                },
                Source = new Source { Name = "Core Rulebook" }
            };

            // Act
            var moveDto = move.ToDTO();

            // Assert
            Assert.IsNotNull(moveDto);
            Assert.AreEqual(move.Id, moveDto.Id);
            Assert.AreEqual(move.Name, moveDto.Name);
            Assert.AreEqual(move.Category, moveDto.Category);
            Assert.AreEqual(move.Text, moveDto.Text);
            Assert.AreEqual(move.Optional, moveDto.Optional);
            Assert.AreEqual(move.ProgressMove, moveDto.ProgressMove);
            Assert.AreEqual(move.VariantOf, moveDto.VariantOf);

            Assert.IsNotNull(moveDto.Trigger);
            Assert.AreEqual(move.Trigger.Text, moveDto.Trigger.Text);

            Assert.IsNotNull(moveDto.Outcomes);
            Assert.IsNotNull(moveDto.Outcomes.StrongHit);
            Assert.AreEqual(move.Outcomes.StrongHit.Text, moveDto.Outcomes.StrongHit.Text);
            Assert.IsNotNull(moveDto.Outcomes.WeakHit);
            Assert.AreEqual(move.Outcomes.WeakHit.Text, moveDto.Outcomes.WeakHit.Text);
            Assert.IsNotNull(moveDto.Outcomes.Miss);
            Assert.AreEqual(move.Outcomes.Miss.Text, moveDto.Outcomes.Miss.Text);

            Assert.IsNotNull(moveDto.Source);
            Assert.AreEqual(move.Source.Name, moveDto.Source.Name);
            Assert.IsNull(moveDto.Source.Url); // As per local Source.ToDTO logic
        }

        [TestMethod]
        public void TestMoveToMoveDTOMapping_NullSource_ShouldReturnNull()
        {
            // Arrange
            Move move = null;

            // Act
            var moveDto = move.ToDTO();

            // Assert
            Assert.IsNull(moveDto);
        }

        [TestMethod]
        public void TestMoveToMoveDTOMapping_NullNestedObjects_ShouldMapToNullDTOs()
        {
            // Arrange
            var move = new Move
            {
                Id = "test_move_id_2",
                Name = "Test Move with Nulls",
                Category = "Test Category 2",
                Text = "Move text.",
                Trigger = null,
                Outcomes = null,
                Source = null
            };

            // Act
            var moveDto = move.ToDTO();

            // Assert
            Assert.IsNotNull(moveDto);
            Assert.AreEqual(move.Id, moveDto.Id);
            Assert.IsNull(moveDto.Trigger);
            Assert.IsNull(moveDto.Outcomes);
            Assert.IsNull(moveDto.Source);
        }
    }
}
