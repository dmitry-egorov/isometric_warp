using WarpSpace.Common.Behaviours;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Game.Battle.Unit
{
    public class WVisibility
    {
        public WVisibility(MUnit the_unit, UnitMeshPresenter the_mesh_presenter)
        {
            its_mesh_presenter = the_mesh_presenter;
            its_unit = the_unit;
        }

        public void Shows() => its_mesh_presenter.Presents(its_unit);
        public void Hides() => its_mesh_presenter.Hides();

        private readonly MUnit its_unit;
        private readonly UnitMeshPresenter its_mesh_presenter;
    }
}