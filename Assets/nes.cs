using UnityEngine;
using Aya.UNES;

public class TestNES : MonoBehaviour
{
    public UNESBehaviour emulator;
    public TextAsset rom;

    void Start()
    {
        Debug.Log("Arrancando ROM...");
        emulator.Boot(rom.bytes);
    }
}