mergeInto(LibraryManager.library, {
  DllGetVKFriends: function()
  {
    GetVKFriends();
  }
});

mergeInto(LibraryManager.library, {
    DllGetOKFriends: function()
  {
    GetOKFriends();
  }
});