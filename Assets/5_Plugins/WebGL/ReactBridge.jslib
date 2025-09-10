mergeInto(LibraryManager.library, {
  GetCurrency: function () {
    if (typeof window.GetCurrency === "function") {
      return window.GetCurrency();
    }
    return 0;
  },

  GetPrizesWon: function () {
    if (typeof window.GetPrizesWon === "function") {
      return window.GetPrizesWon();
    }
    return 0;
  },

  UpdateCurrencyFromUnity: function (valuePtr) {
    const value = UTF8ToString(valuePtr);
    if (typeof window.UpdateCurrencyFromUnity === "function") {
      window.UpdateCurrencyFromUnity(value);
    }
  },

  UpdatePrizesFromUnity: function (valuePtr) {
    const value = UTF8ToString(valuePtr);
    if (typeof window.UpdatePrizesFromUnity === "function") {
      window.UpdatePrizesFromUnity(value);
    }
  },

  TrySpendCurrencyFromUnity: function (amount) {
    if (typeof window.TrySpendCurrency === "function") {
      return window.TrySpendCurrency(amount);
    }
    console.error("TrySpendCurrency not defined on window");
    return 0;
  }
});