using TheOracle2.Data; // For Move, Trigger, Outcomes, Outcome, Source
using Server.GameInterfaces.DTOs; // For MoveDTO and its constituent DTOs
// System.Linq might be needed for other methods if they involve list projections.

namespace Server.GameInterfaces.DTOs
{
    public static class MoveExtensions
    {
        // Extension method for TheOracle2.Data.Outcome
        public static MoveOutcomeDTO ToDTO(this Outcome outcome) // Assuming TheOracle2.Data.Outcome
        {
            if (outcome == null)
            {
                return null;
            }

            return new MoveOutcomeDTO
            {
                Text = outcome.Text
            };
        }

        // Extension method for TheOracle2.Data.Outcomes
        public static MoveOutcomesDTO ToDTO(this Outcomes outcomes) // Assuming TheOracle2.Data.Outcomes
        {
            if (outcomes == null)
            {
                return null;
            }

            return new MoveOutcomesDTO
            {
                StrongHit = outcomes.StrongHit?.ToDTO(), // Uses the Outcome.ToDTO() above
                WeakHit = outcomes.WeakHit?.ToDTO(),   // Uses the Outcome.ToDTO() above
                Miss = outcomes.Miss?.ToDTO()          // Uses the Outcome.ToDTO() above
            };
        }

        // Extension method for TheOracle2.Data.Trigger
        public static MoveTriggerDTO ToDTO(this Trigger trigger) // Assuming TheOracle2.Data.Trigger
        {
            if (trigger == null)
            {
                return null;
            }

            return new MoveTriggerDTO
            {
                Text = trigger.Text
            };
        }

        // Extension method for TheOracle2.Data.Source (simplified for moves)
        public static SourceDTO ToDTO(this Source source) // Assuming TheOracle2.Data.Source
        {
            if (source == null)
            {
                return null;
            }
            return new SourceDTO
            {
                Name = source.Name,
                Url = null // SourceDTO for moves does not require URL as per previous DTO definitions
            };
        }

        // Extension method for TheOracle2.Data.Move
        public static MoveDTO ToDTO(this Move move)
        {
            if (move == null)
            {
                return null;
            }

            return new MoveDTO
            {
                Id = move.Id,
                Name = move.Name,
                Category = move.Category,
                Text = move.Text,
                Trigger = move.Trigger?.ToDTO(),     // Uses Trigger.ToDTO()
                Outcomes = move.Outcomes?.ToDTO(),   // Uses Outcomes.ToDTO()
                Source = move.Source?.ToDTO(),       // Uses Source.ToDTO() defined above
                Optional = move.Optional,
                ProgressMove = move.ProgressMove,
                VariantOf = move.VariantOf
            };
        }
    }
}
