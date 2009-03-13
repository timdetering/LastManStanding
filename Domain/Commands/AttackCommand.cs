using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.Movement;

namespace LastManStanding.Domain.Commands
{
    public class AttackCommand : ICommand
    {
        private readonly IActor attacker;
        private readonly IActor defender;

        public AttackCommand(IActor attacker, IActor defender)
        {
            this.attacker = attacker;
            this.defender = defender;
        }

        public CommandResult Execute()
        {

            // Get the weapon / damage from the attacker

            // We use the attacker damage

            // Determine if the attack hits

            // We assume the attack will always hit

            // Apply the damage

            defender.Health -= attacker.Damage;
            var result = new CommandResult() {Name = "AttackCommand", UsesTurn = true, Success = true, Message = string.Format("{0} hits {1} for {2} damage", attacker, defender, attacker.Damage)};
            
            if (!defender.IsAlive) attacker.AddKill(defender);

            return result;
        }
    }
}
