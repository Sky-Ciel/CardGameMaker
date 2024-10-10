using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;
using DG.Tweening;

public class FileSearchDialog : MonoBehaviour
{
    [SerializeField] private GameObject dialogWindow;
    [SerializeField] private Transform fileListContent;
    [SerializeField] private GameObject fileItemPrefab;
    [SerializeField] private Button selectButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_InputField pathInput;
    [SerializeField] private TMP_Text currentPathText;
    [SerializeField] private CanvasGroup canvasGroup;

    private string currentPath;
    private string selectedFilePath;
    private List<FileItem> fileItems = new List<FileItem>();

    private void Start()
    {
        selectButton.onClick.AddListener(OnSelect);
        cancelButton.onClick.AddListener(OnCancel);
        SetupFonts();
    }

    public void OpenFileDialog()
    {
        currentPath = Directory.GetCurrentDirectory();
        UpdateFileList();
        FadeIn();
    }

    private void UpdateFileList()
    {
        ClearFileList();
        currentPathText.text = currentPath;

        string[] directories = Directory.GetDirectories(currentPath);
        string[] files = Directory.GetFiles(currentPath);

        foreach (string directory in directories)
        {
            CreateFileItem(Path.GetFileName(directory), true);
        }

        foreach (string file in files)
        {
            CreateFileItem(Path.GetFileName(file), false);
        }
    }

    private void CreateFileItem(string name, bool isDirectory)
    {
        GameObject item = Instantiate(fileItemPrefab, fileListContent);
        FileItem fileItem = item.GetComponent<FileItem>();
        fileItem.Initialize(name, isDirectory);
        fileItem.OnClick += OnFileItemClick;
        fileItems.Add(fileItem);
    }

    private void ClearFileList()
    {
        foreach (FileItem item in fileItems)
        {
            Destroy(item.gameObject);
        }
        fileItems.Clear();
    }

    private void OnFileItemClick(FileItem item)
    {
        if (item.IsDirectory)
        {
            currentPath = Path.Combine(currentPath, item.FileName);
            UpdateFileList();
        }
        else
        {
            selectedFilePath = Path.Combine(currentPath, item.FileName);
            pathInput.text = selectedFilePath;
            VisualizeSelection(item);
        }
    }

    private void VisualizeSelection(FileItem selectedItem)
    {
        foreach (FileItem item in fileItems)
        {
            item.SetSelected(item == selectedItem);
        }
    }

    private void OnSelect()
    {
        if (!string.IsNullOrEmpty(selectedFilePath))
        {
            Debug.Log("Selected file: " + selectedFilePath);
            FadeOut(() => gameObject.SetActive(false));
        }
    }

    private void OnCancel()
    {
        FadeOut(() => gameObject.SetActive(false));
    }

    private void FadeIn()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.3f).SetEase(Ease.OutQuad);
    }

    private void FadeOut(TweenCallback onComplete)
    {
        canvasGroup.DOFade(0f, 0.3f).SetEase(Ease.InQuad).OnComplete(onComplete);
    }

    private void SetupFonts()
    {
        TMP_FontAsset customFont = Resources.Load<TMP_FontAsset>("Fonts/YourCustomFont");
        if (customFont != null)
        {
            TextMeshProUGUI[] tmpTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI tmp in tmpTexts)
            {
                tmp.font = customFont;
            }
        }
    }
}

public class FileItem : MonoBehaviour
{
    [SerializeField] private TMP_Text fileNameText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;
    [SerializeField] private Image backgroundImage;

    public string FileName { get; private set; }
    public bool IsDirectory { get; private set; }
    public event System.Action<FileItem> OnClick;

    private Color normalColor = new Color(0.9f, 0.9f, 0.9f);
    private Color selectedColor = new Color(0.7f, 0.8f, 1f);

    private void Start()
    {
        button.onClick.AddListener(() => OnClick?.Invoke(this));
    }

    public void Initialize(string fileName, bool isDirectory)
    {
        FileName = fileName;
        IsDirectory = isDirectory;
        fileNameText.text = fileName;
        iconImage.sprite = isDirectory ? Resources.Load<Sprite>("Icons/FolderIcon") : Resources.Load<Sprite>("Icons/FileIcon");
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        backgroundImage.color = selected ? selectedColor : normalColor;
    }
}