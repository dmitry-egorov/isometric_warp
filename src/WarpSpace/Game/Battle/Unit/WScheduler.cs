using System;

namespace WarpSpace.Game.Battle.Unit
{
    public class WScheduler
    {
        public WScheduler(WUnit the_owner, WAgenda the_agenda)
        {
            this.its_owner = the_owner;
            this.the_agenda = the_agenda;
            its_wiring = it_wires_units_movements();
        }
        
        public void Destructs() => it_destructs();
        
        private Action it_wires_units_movements() => its_owner.s_Unit.Moved.Subscribe(x => the_agenda.Enqueues_a_Move(x.Source, x.Destination));
        private void it_destructs() => its_wiring();

        private readonly WUnit its_owner;
        private readonly WAgenda the_agenda;
        private readonly Action its_wiring;
    }
}