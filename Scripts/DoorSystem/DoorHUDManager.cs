using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SPACE_UTIL;

namespace SPACE_GAME
{
	/// <summary>
	/// Singleton manager for door interaction HUD (screen-space overlay).
	/// Shows prompts like "E to Open", "Locked - Red Key Required", etc.
	/// Attach to a Canvas GameObject in scene.
	/// </summary>
	public class DoorHUDManager : MonoBehaviour
	{
		// ===== SINGLETON ===== //
		public static DoorHUDManager Instance { get; private set; }

		// ===== SERIALIZED FIELDS ===== //
		[Header("HUD References")]
		[SerializeField] private GameObject hudPanel; // Container for HUD elements
		[SerializeField] private TextMeshProUGUI promptText;
		[SerializeField] private Image promptIcon;

		[Header("Icons")]
		[SerializeField] private Sprite iconHand; // Generic interaction
		[SerializeField] private Sprite iconLock; // Locked door
		[SerializeField] private Sprite iconWarning; // Swaying door

		[Header("Settings")]
		[SerializeField] private float fadeSpeed = 5f;

		// ===== PRIVATE FIELDS ===== //
		private CanvasGroup canvasGroup;
		private bool isShowing = false;

		// ===== UNITY LIFECYCLE ===== //

		private void Awake()
		{
			// Singleton pattern
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}
			Instance = this;

			// Get or add CanvasGroup for fading
			canvasGroup = hudPanel.GetComponent<CanvasGroup>();
			if (canvasGroup == null)
				canvasGroup = hudPanel.AddComponent<CanvasGroup>();

			// Start hidden
			canvasGroup.alpha = 0f;
			hudPanel.SetActive(false);
		}

		private void Update()
		{
			// Fade in/out
			float targetAlpha = isShowing ? 1f : 0f;
			canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);

			// Deactivate when fully faded out
			if (canvasGroup.alpha < 0.01f && !isShowing)
			{
				hudPanel.SetActive(false);
			}
		}

		// ===== PUBLIC API ===== //

		/// <summary>
		/// Show interaction prompt for a door.
		/// </summary>
		public void ShowPrompt(IDoor door, string actionText, PromptIcon icon = PromptIcon.Hand)
		{
			if (!isShowing)
			{
				hudPanel.SetActive(true);
				isShowing = true;
			}

			// Update text
			if (promptText != null)
				promptText.text = actionText;

			// Update icon
			if (promptIcon != null)
			{
				switch (icon)
				{
					case PromptIcon.Hand:
						promptIcon.sprite = iconHand;
						break;
					case PromptIcon.Lock:
						promptIcon.sprite = iconLock;
						break;
					case PromptIcon.Warning:
						promptIcon.sprite = iconWarning;
						break;
				}
				promptIcon.gameObject.SetActive(true);
			}
		}

		/// <summary>
		/// Hide the HUD prompt.
		/// </summary>
		public void HidePrompt()
		{
			isShowing = false;
		}

		/// <summary>
		/// Quick helpers for common prompts.
		/// </summary>
		public void ShowOpenPrompt(IDoor door)
		{
			ShowPrompt(door, "E to Open", PromptIcon.Hand);
		}

		public void ShowClosePrompt(IDoor door)
		{
			ShowPrompt(door, "E to Close", PromptIcon.Hand);
		}

		public void ShowLockedPrompt(IDoor door, string keyName = "Key")
		{
			ShowPrompt(door, $"Locked - {keyName} Required", PromptIcon.Lock);
		}

		public void ShowOpeningPrompt()
		{
			ShowPrompt(null, "Door Opening...", PromptIcon.Hand);
		}

		public void ShowClosingPrompt()
		{
			ShowPrompt(null, "Door Closing...", PromptIcon.Hand);
		}
	}

	// ===== ENUMS ===== //

	public enum PromptIcon
	{
		Hand,    // Generic interaction
		Lock,    // Locked door
		Warning, // Swaying door
	}
}