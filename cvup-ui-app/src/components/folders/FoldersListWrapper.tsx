import { observer } from "mobx-react";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { FoldersList } from "./FoldersList";

export const FoldersListWrapper = observer(() => {
  const { foldersStore } = useStore();

  useEffect(() => {
    if (!foldersStore.foldersList.length) {
      foldersStore.getFoldersList();
    }
  }, []);

  return <FoldersList />;
});
