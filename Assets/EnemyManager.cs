using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class EnemyManager : NetworkBehaviour
{
    public readonly SyncVar<float> redHealth = new SyncVar<float>(100);
    public readonly SyncVar<float> greenHealth = new SyncVar<float>(100);
    public readonly SyncVar<float> blueHealth = new SyncVar<float>(100);
    public float maxHealth = 100;
    public float startingRedHealth;
    public float startingGreenHealth;
    public float startingBlueHealth;

    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;
    public Slider magentaSlider;
    public Slider cyanSlider;
    public Slider yellowSlider;
    public Slider whiteSlider;

    // Start is called before the first frame update
    public void Start()
    {
        redHealth.Value = startingRedHealth;
        blueHealth.Value = startingBlueHealth;
        greenHealth.Value = startingGreenHealth;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DamageEnemy(int damage, string color)
    {
        if (color == "red")
        {
            print("Hit with color " + color + " health is " + redHealth.Value.ToString());
            redHealth.Value -= damage;
            if (redHealth.Value < 0)
            {
                redHealth.Value = 0;
            }
        }
        if (color == "green")
        {
            print("Hit with color " + color + " health is " + greenHealth.Value.ToString());
            greenHealth.Value -= damage;
            if (greenHealth.Value < 0)
            {
                greenHealth.Value = 0;
            }
        }
        if (color == "blue")
        {
            print("Hit with color " + color + " health is " + blueHealth.Value.ToString());
            blueHealth.Value -= damage;
            if (blueHealth.Value < 0)
            {
                blueHealth.Value = 0;
            }
        }
        if (redHealth.Value <= 0 && greenHealth.Value <= 0 && blueHealth.Value <= 0)
        {
            Despawn();
        }
    }

    [ObserversRpc]
    public void Despawn()
    {
        gameObject.SetActive(false);
    }

    //show local value on the healthbars
    public void updateHealthBar()
    {
        redSlider.value = redHealth.Value / maxHealth;
        greenSlider.value = greenHealth.Value / maxHealth;
        blueSlider.value = blueHealth.Value / maxHealth;
        magentaSlider.value = Mathf.Max(0f, Mathf.Min(redHealth.Value, blueHealth.Value)) / maxHealth;
        cyanSlider.value = Mathf.Max(0f, Mathf.Min(greenHealth.Value, blueHealth.Value)) / maxHealth;
        yellowSlider.value = Mathf.Max(0f, Mathf.Min(redHealth.Value, greenHealth.Value)) / maxHealth;
        whiteSlider.value = Mathf.Min(redHealth.Value, blueHealth.Value, greenHealth.Value) / maxHealth;
    }

    //if undamaged for a time, regens health for the lower color healthbars. (If redhealth = 10, blue health = 8, then bluehealth will regenerate, and the healthbar color will be megenta again)
    public void regenHealth()
    {

    }

    public void Update() {
        updateHealthBar();
        regenHealth();
    }
}
