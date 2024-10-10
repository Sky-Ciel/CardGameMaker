using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using System.Threading.Tasks;

namespace FileDialogWindows
{
    public class FileDialog : MonoBehaviour
    {
        private Canvas canvas;
        private CanvasScaler canvasScaler;
        private GraphicRaycaster graphicRaycaster;
        private CanvasGroup canvasGroup;
        
        private RectTransform windowRect;
        private ScrollRect scrollView;
        private TMP_InputField searchInput;
        private Button confirmButton;
        private Button cancelButton;
        private Button backButton;
        private TextMeshProUGUI currentPathText;
        
        private string currentPath;
        private string selectedFilePath;
        private string[] allowedExtensions;
        
        [SerializeField] private TMP_FontAsset customFont;
        [SerializeField] private Vector2 referenceResolution = new Vector2(1920, 1080);
        
        private Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.95f);
        private Color headerColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        private Color buttonColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        private Color textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        private Color selectedColor = new Color(0.4f, 0.6f, 1f, 1f);

        private TaskCompletionSource<string> dialogCompletionSource;

        private void Awake()
        {
            SetupCanvas();
            CreateUIElements();
            SetupEventListeners();
            
            // Initially hide the dialog
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        
        private void SetupCanvas()
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;  // Ensure it's on top of other UI elements
            
            canvasScaler = gameObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = referenceResolution;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;
            
            graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
            
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        private void CreateUIElements()
        {
            // Create window
            windowRect = CreateRectTransform("Window", transform);
            windowRect.sizeDelta = new Vector2(800, 600);
            CreateImage("WindowBackground", windowRect, backgroundColor);
            
            // Create header
            var headerRect = CreateRectTransform("Header", windowRect);
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.sizeDelta = new Vector2(0, 50);
            headerRect.anchoredPosition = new Vector2(0, -25);
            CreateImage("HeaderBackground", headerRect, headerColor);
            
            // Create title
            var titleText = CreateText("Title", headerRect, "File Selection", 24, TextAlignmentOptions.Center);
            titleText.rectTransform.anchoredPosition = new Vector2(0, 0);
            
            // Create back button
            backButton = CreateButton("BackButton", headerRect, "←");
            backButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, 0);
            backButton.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
            
            // Create current path text
            currentPathText = CreateText("CurrentPath", headerRect, "", 16, TextAlignmentOptions.Left);
            currentPathText.rectTransform.anchorMin = new Vector2(0, 0);
            currentPathText.rectTransform.anchorMax = new Vector2(1, 1);
            currentPathText.rectTransform.offsetMin = new Vector2(60, 5);
            currentPathText.rectTransform.offsetMax = new Vector2(-10, -5);
            
            // Create search input
            searchInput = CreateInputField("SearchInput", windowRect, "Search...");
            searchInput.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            searchInput.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            searchInput.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);
            searchInput.GetComponent<RectTransform>().sizeDelta = new Vector2(-20, 40);
            
            // Create scroll view for file list
            scrollView = CreateScrollView("FileList", windowRect);
            scrollView.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            scrollView.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            scrollView.GetComponent<RectTransform>().offsetMin = new Vector2(10, 60);
            scrollView.GetComponent<RectTransform>().offsetMax = new Vector2(-10, -120);
            
            // Create buttons
            confirmButton = CreateButton("ConfirmButton", windowRect, "Confirm");
            confirmButton.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0);
            confirmButton.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);
            confirmButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-60, 20);
            confirmButton.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
            
            cancelButton = CreateButton("CancelButton", windowRect, "Cancel");
            cancelButton.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            cancelButton.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
            cancelButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(60, 20);
            cancelButton.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
        }
        
        private void SetupEventListeners()
        {
            searchInput.onValueChanged.AddListener(OnSearchValueChanged);
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
        
        public async Task<string> OpenFileDialog(string[] extensions = null)
        {
            allowedExtensions = extensions;
            currentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            UpdateFileList();
            
            dialogCompletionSource = new TaskCompletionSource<string>();

            // Show and animate the dialog
            canvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutQuad);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            windowRect.DOScale(Vector3.one, 0.3f).From(Vector3.zero).SetEase(Ease.OutBack);
            EnsureDialogFitsScreen();
            return await dialogCompletionSource.Task;
        }
        
        private void UpdateFileList()
        {
            // Clear existing items
            foreach (Transform child in scrollView.content)
            {
                Destroy(child.gameObject);
            }
            
            currentPathText.text = currentPath;
            
            try
            {
                // Get directories and files
                var directories = Directory.GetDirectories(currentPath);
                var files = Directory.GetFiles(currentPath)
                    .Where(f => allowedExtensions == null || allowedExtensions.Contains(Path.GetExtension(f).ToLower()))
                    .ToArray();
                
                CreateListItems(directories, true);
                CreateListItems(files, false);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error accessing directory {currentPath}: {e.Message}");
                currentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                UpdateFileList();
            }
        }
        
        private void CreateListItems(string[] paths, bool isDirectory)
        {
            foreach (var path in paths)
            {
                var item = CreateButton($"Item_{Path.GetFileName(path)}", scrollView.content, Path.GetFileName(path));
                var itemRect = item.GetComponent<RectTransform>();
                itemRect.sizeDelta = new Vector2(0, 30);
                
                var icon = CreateImage($"Icon_{Path.GetFileName(path)}", item.transform, isDirectory ? Color.yellow : Color.white);
                icon.rectTransform.anchorMin = new Vector2(0, 0.5f);
                icon.rectTransform.anchorMax = new Vector2(0, 0.5f);
                icon.rectTransform.anchoredPosition = new Vector2(10, 0);
                icon.rectTransform.sizeDelta = new Vector2(20, 20);
                
                var itemText = item.GetComponentInChildren<TextMeshProUGUI>();
                itemText.alignment = TextAlignmentOptions.Left;
                itemText.fontSize = 14;
                itemText.rectTransform.offsetMin = new Vector2(35, 0);
                
                item.onClick.AddListener(() => OnItemClicked(path, isDirectory));
            }
            
            // Update content size
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollView.content);
        }
        
        private void OnItemClicked(string path, bool isDirectory)
        {
            if (isDirectory)
            {
                currentPath = path;
                UpdateFileList();
            }
            else
            {
                selectedFilePath = path;
                HighlightSelectedFile(path);
            }
            UpdateConfirmButtonState();
        }
        
        private void HighlightSelectedFile(string path)
        {
            foreach (Transform child in scrollView.content)
            {
                var button = child.GetComponent<Button>();
                var colors = button.colors;
                colors.normalColor = (child.name == $"Item_{Path.GetFileName(path)}") ? selectedColor : buttonColor;
                button.colors = colors;
            }
        }
        
        private void OnSearchValueChanged(string value)
        {
            foreach (Transform child in scrollView.content)
            {
                child.gameObject.SetActive(child.name.ToLower().Contains(value.ToLower()));
            }
        }
        
        private void OnConfirmButtonClicked()
        {
            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                CloseDialog(selectedFilePath);
            }
        }
        
        private void OnCancelButtonClicked()
        {
            CloseDialog(null);
        }
        
        private void OnBackButtonClicked()
        {
            var parentDirectory = Directory.GetParent(currentPath);
            if (parentDirectory != null)
            {
                currentPath = parentDirectory.FullName;
                UpdateFileList();
            }
        }
        
        private void CloseDialog(string result)
        {
            // Animate and hide the dialog
            canvasGroup.DOFade(0, 0.3f).SetEase(Ease.InQuad);
            windowRect.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
                .OnComplete(() => 
                {
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                    dialogCompletionSource.SetResult(result);
                });
        }
        
        // Helper methods for creating UI elements
        private RectTransform CreateRectTransform(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rectTransform = go.GetComponent<RectTransform>();
            rectTransform.SetParent(parent, false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            return rectTransform;
        }

        private Image CreateImage(string name, Transform parent, Color color)
        {
            var image = new GameObject(name, typeof(Image)).GetComponent<Image>();
            image.transform.SetParent(parent, false);
            image.color = color;
            return image;
        }

        private TextMeshProUGUI CreateText(string name, Transform parent, string text, int fontSize, TextAlignmentOptions alignment)
        {
            var textObj = new GameObject(name, typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            textObj.transform.SetParent(parent, false);
            textObj.text = text;
            textObj.fontSize = fontSize;
            textObj.alignment = alignment;
            textObj.color = textColor;
            textObj.font = customFont;
            return textObj;
        }

        private TMP_InputField CreateInputField(string name, Transform parent, string placeholder)
        {
            var inputFieldObj = new GameObject(name, typeof(Image), typeof(TMP_InputField));
            var inputField = inputFieldObj.GetComponent<TMP_InputField>();
            inputFieldObj.transform.SetParent(parent, false);
            
            var textArea = CreateRectTransform("TextArea", inputFieldObj.transform);
            var placeholderObj = CreateText("Placeholder", textArea, placeholder, 16, TextAlignmentOptions.Left);
            var textComponent = CreateText("Text", textArea, "", 16, TextAlignmentOptions.Left);
            
            inputField.textViewport = textArea;
            inputField.textComponent = textComponent;
            inputField.placeholder = placeholderObj;
            inputField.image.color = new Color(0.9f, 0.9f, 0.9f, 0.1f);
            
            return inputField;
        }

        private ScrollRect CreateScrollView(string name, Transform parent)
        {
            var scrollViewObj = new GameObject(name, typeof(Image), typeof(ScrollRect));
            var scrollRect = scrollViewObj.GetComponent<ScrollRect>();
            scrollViewObj.transform.SetParent(parent, false);
            
            var viewport = CreateRectTransform("Viewport", scrollRect.transform);
            viewport.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            viewport.gameObject.AddComponent<Mask>();
            viewport.gameObject.AddComponent<Image>().color = new Color(1, 1, 1, 0.05f);
            
            var content = CreateRectTransform("Content", viewport);
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
            content.sizeDelta = new Vector2(0, 0);
            content.anchoredPosition = Vector2.zero;
            
            scrollRect.content = content;
            scrollRect.viewport = viewport;
            scrollRect.horizontal = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 30;
            
            var verticalScrollbar = CreateScrollbar("VerticalScrollbar", scrollRect.transform);
            scrollRect.verticalScrollbar = verticalScrollbar;
            
            var verticalScrollbarRect = verticalScrollbar.GetComponent<RectTransform>();
            verticalScrollbarRect.anchorMin = new Vector2(1, 0);
            verticalScrollbarRect.anchorMax = new Vector2(1, 1);
            verticalScrollbarRect.sizeDelta = new Vector2(10, 0);
            verticalScrollbarRect.anchoredPosition = new Vector2(-5, 0);
            
            return scrollRect;
        }

        private Scrollbar CreateScrollbar(string name, Transform parent)
        {
            var scrollbarObj = new GameObject(name, typeof(Image), typeof(Scrollbar));
            var scrollbar = scrollbarObj.GetComponent<Scrollbar>();
            scrollbarObj.transform.SetParent(parent, false);
            
            var slidingArea = CreateRectTransform("SlidingArea", scrollbar.transform);
            slidingArea.sizeDelta = Vector2.zero;
            
            var handle = CreateImage("Handle", slidingArea, new Color(0.7f, 0.7f, 0.7f, 0.7f));
            handle.rectTransform.sizeDelta = Vector2.zero;
            
            scrollbar.handleRect = handle.rectTransform;
            scrollbar.targetGraphic = handle;
            scrollbar.direction = Scrollbar.Direction.BottomToTop;
            
            return scrollbar;
        }

        private Button CreateButton(string name, Transform parent, string text)
        {
            var buttonObj = new GameObject(name, typeof(Image), typeof(Button));
            var button = buttonObj.GetComponent<Button>();
            buttonObj.transform.SetParent(parent, false);
            
            var textComponent = CreateText($"{name}Text", button.transform, text, 14, TextAlignmentOptions.Center);
            textComponent.rectTransform.sizeDelta = Vector2.zero;
            
            var colors = button.colors;
            colors.normalColor = buttonColor;
            colors.highlightedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
            colors.selectedColor = selectedColor;
            button.colors = colors;
            
            return button;
        }

        private void SetButtonInteractable(Button button, bool interactable)
        {
            button.interactable = interactable;
            var textColor = interactable ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : new Color(0.5f, 0.5f, 0.5f, 1f);
            button.GetComponentInChildren<TextMeshProUGUI>().color = textColor;
        }

        private void UpdateConfirmButtonState()
        {
            SetButtonInteractable(confirmButton, !string.IsNullOrEmpty(selectedFilePath));
        }

        private void OnEnable()
        {
            // Register to back button (Android) or escape key (desktop) events
            Input.backButtonLeavesApp = false;
        }

        private void OnDisable()
        {
            // Unregister from back button events
            Input.backButtonLeavesApp = true;
        }

        private void Update()
        {
            // Handle back button or escape key
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnBackButtonClicked();
            }
        }

        // Public method to set allowed file extensions
        public void SetAllowedExtensions(string[] extensions)
        {
            allowedExtensions = extensions;
            if (gameObject.activeInHierarchy)
            {
                UpdateFileList();
            }
        }

        // Public method to set the initial directory
        public void SetInitialDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                currentPath = path;
                if (gameObject.activeInHierarchy)
                {
                    UpdateFileList();
                }
            }
            else
            {
                Debug.LogWarning($"Directory does not exist: {path}");
            }
        }

        // Public method to customize colors
        public void SetColors(Color bgColor, Color headerColor, Color btnColor, Color txtColor, Color selColor)
        {
            backgroundColor = bgColor;
            this.headerColor = headerColor;
            buttonColor = btnColor;
            textColor = txtColor;
            selectedColor = selColor;

            // Update existing UI elements
            if (gameObject.activeInHierarchy)
            {
                UpdateUIColors();
            }
        }

        private void UpdateUIColors()
        {
            // Update background color
            var windowBg = windowRect.GetComponent<Image>();
            if (windowBg != null) windowBg.color = backgroundColor;

            // Update header color
            var headerBg = windowRect.Find("Header/HeaderBackground").GetComponent<Image>();
            if (headerBg != null) headerBg.color = headerColor;

            // Update button colors
            UpdateButtonColors(confirmButton);
            UpdateButtonColors(cancelButton);
            UpdateButtonColors(backButton);

            // Update text colors
            UpdateTextColors(windowRect);

            // Update scroll view colors
            var scrollViewBg = scrollView.GetComponent<Image>();
            if (scrollViewBg != null) scrollViewBg.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0.05f);

            // Force layout rebuild
            LayoutRebuilder.ForceRebuildLayoutImmediate(windowRect);
        }

        private void UpdateButtonColors(Button button)
        {
            var colors = button.colors;
            colors.normalColor = buttonColor;
            colors.highlightedColor = Color.Lerp(buttonColor, Color.white, 0.1f);
            colors.pressedColor = Color.Lerp(buttonColor, Color.black, 0.1f);
            colors.selectedColor = selectedColor;
            button.colors = colors;
        }

        private void UpdateTextColors(Transform parent)
        {
            var texts = parent.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var text in texts)
            {
                text.color = textColor;
            }
        }

        // Method to ensure the dialog fits within the screen
        private void EnsureDialogFitsScreen()
        {
            var safeArea = Screen.safeArea;
            var screenRatio = safeArea.width / safeArea.height;

            float dialogWidth = Mathf.Min(800, safeArea.width * 0.9f);
            float dialogHeight = Mathf.Min(600, safeArea.height * 0.9f);

            windowRect.sizeDelta = new Vector2(dialogWidth, dialogHeight);
            windowRect.anchoredPosition = Vector2.zero;
        }

        // Override the OpenFileDialog method to ensure proper sizing
        /*public new async Task<string> OpenFileDialog(string[] extensions = null)
        {
            EnsureDialogFitsScreen();
            return await base.OpenFileDialog(extensions);
        }*/
    }
}