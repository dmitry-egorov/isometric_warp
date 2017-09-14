using UnityEngine;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Settings
{
    [CreateAssetMenu(fileName = "UnitType", menuName = "Custom/Unit Type", order = 1)]
    public class UnitTypeSettings : SettingsObject<MUnitType>
    {
        public UnitTypeModelSettings Model;
        public MovementSettings Movement;
        public Mesh Mesh;
        
        public override MUnitType Creates_a_Model() => Model.s_Description();
    }
}