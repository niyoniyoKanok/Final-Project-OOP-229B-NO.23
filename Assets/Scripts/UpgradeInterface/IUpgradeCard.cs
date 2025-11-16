using UnityEngine;
    public interface IUpgradeCard
    {
        
        string GetTitle();

        string GetDescription();


        void ApplyUpgrade(GameObject playerObject);
    }
