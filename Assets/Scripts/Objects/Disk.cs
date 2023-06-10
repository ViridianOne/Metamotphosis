using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disk : MonoBehaviour, IDataPersistance
{
    public TextMesh textMesh;
    private float timer;
    [SerializeField] private Location location;
    private Animator anim;

    private bool isCollected = false;
    [SerializeField] private string id;
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

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
        if (!isCollected && collision.CompareTag("Player"))
        {
            isCollected = true;
            LevelManager.instance.CollectDisk();
            timer = 5f;
            if (textMesh.text != null)
            {
                textMesh.text = $"Disks collected: {LevelManager.instance.disksCount} / {LevelManager.instance.maxDisksAmount}";
            }
            gameObject.GetComponent<Collider2D>().enabled = !gameObject.GetComponent<Collider2D>().enabled;
            gameObject.GetComponent<SpriteRenderer>().enabled = !gameObject.GetComponent<SpriteRenderer>().enabled;
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
            //gameObject.SetActive(false);
            StartCoroutine(DestroyDisk());
        }
    }

    private IEnumerator DestroyDisk()
    {
        yield return new WaitForSeconds(timer);
        gameObject.SetActive(false);
    }

    public void LoadData(GameData data)
    {
        data.collectedDisks.TryGetValue(id, out isCollected);
        if (isCollected)
        {
            gameObject.SetActive(false);
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.collectedDisks.ContainsKey(id))
        {
            data.collectedDisks.Remove(id);
        }
        data.collectedDisks.Add(id, isCollected);
    }
}
