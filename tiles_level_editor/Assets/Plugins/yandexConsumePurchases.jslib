mergeInto(LibraryManager.library, {
	ConsumePurchasesInternal: function()
	{
		ConsumePurchases();
	}
});

mergeInto(LibraryManager.library, {
	ConsumeYandexInternal: function()
	{
		ConsumeYandex();
	},

	OnCloseApp: function () {
		window.onunload = function (event) {
			window.unityInstance.SendMessage("Managers/DataAccount", "OnCloseApp");
		};
	}
});