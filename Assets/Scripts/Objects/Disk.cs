using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disk : MonoBehaviour
{
    public TextMesh textMesh;
    private float timer;
    [SerializeField] private Location location;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        anim.SetFloat("diskNumber", (float)location);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LevelManager.instance.CollectDick();
            timer = 5f;
            if (textMesh.text != null)
            {
                textMesh.text = $"Disks collected: {LevelManager.instance.disksCount} / {LevelManager.instance.maxDisksAmount}";
            }
            gameObject.GetComponent<Collider2D>().enabled = !gameObject.GetComponent<Collider2D>().enabled;
            gameObject.GetComponent<SpriteRenderer>().enabled = !gameObject.GetComponent<SpriteRenderer>().enabled;
            //gameObject.SetActive(false);
            StartCoroutine(DestroyDisk());
        }
    }

    private IEnumerator DestroyDisk()
    {
        yield return new WaitForSeconds(timer);
        gameObject.SetActive(false);
    }
}
