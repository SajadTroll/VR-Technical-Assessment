using UnityEngine;

public interface ICollectible
{
    int CollectValue { get; }
    void Collect(GameObject collector);
    void Initialize();
}
