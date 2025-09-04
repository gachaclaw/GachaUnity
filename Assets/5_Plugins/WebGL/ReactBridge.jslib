mergeInto(LibraryManager.library, {
  GetCurrency: function () {
    if (typeof window.GetCurrency === "function") {
      return window.GetCurrency();
    }
    return 0;
  },

  UpdateCurrencyFromUnity: function (valuePtr) {
    const value = UTF8ToString(valuePtr);
    if (typeof window.UpdateCurrencyFromUnity === "function") {
      window.UpdateCurrencyFromUnity(value);
    }
  },

  // Add other bridge methods here as needed
});