using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace TowerDefenseRemake.Constructable
{
    public static class TurretUpgradeCalcurator
    {
        public static float CalcurateNormal(ConstructableInfo info, ParamType type, int nextlevel)
        {
            float levelpercentage = (float)nextlevel / (float)info.Max[type].Level;
            float initialvalue = info.InitialParam[type].ParamValue.Value;
            float maxvalue = info.Max[type].ParamValue.Value;
            float ratio = info.IncreaseRate[type].Ratio;
            float pow = info.IncreaseRate[type].Pow;

            if (maxvalue.ToString() == "Infinity")
            {
                return initialvalue * Mathf.Pow(1 + nextlevel * ratio, pow);
            }
            else
            {
                return initialvalue +
                    (ratio * (maxvalue - initialvalue) * levelpercentage +
                    (1 - ratio) * (maxvalue - initialvalue) * Mathf.Pow(levelpercentage, pow));
            }
        }
    }
}