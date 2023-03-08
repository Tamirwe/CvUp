import { IconButton } from "@mui/material";
import { observer } from "mobx-react-lite";
import { CiEdit } from "react-icons/ci";
import { useStore } from "../../Hooks/useStore";
import { CrudTypesEnum, TabsCandsEnum } from "../../models/GeneralEnums";
import { IFolder, IFolderNode } from "../../models/GeneralModels";
import styles from "./FoldersList.module.scss";
import { MdPersonAddAlt1 } from "react-icons/md";

export const FoldersList = observer(() => {
  const { foldersStore, candsStore, generalStore } = useStore();
  // let vv = 1;

  const editFolder = (folder: IFolder) => {
    return (
      <div className={styles.iconsDiv}>
        <IconButton
          size="small"
          onClick={async () => {
            foldersStore.editFolderSelected = folder;
            generalStore.openModeFolderFormDialog = CrudTypesEnum.Update;
          }}
        >
          <CiEdit />
        </IconButton>
        <IconButton
          size="small"
          onClick={async () => {
            await foldersStore.attachCandidate(folder.id);
          }}
        >
          <MdPersonAddAlt1 />
        </IconButton>
      </div>
    );
  };

  const renderChildren = (node: IFolderNode) => {
    return (
      <li
        key={node.folder.id}
        role="treeitem"
        style={{ cursor: "pointer", lineHeight: 1 }}
      >
        <div
          className={styles.folderLine}
          onClick={() => {
            foldersStore.selectedFolder = node.folder;
            candsStore.getFolderCandsList(node.folder.id);
            candsStore.currentTabCandsList = TabsCandsEnum.FolderCands;
          }}
        >
          <div>{node.folder.name}</div>
          <div className={styles.editFolder}>
            {" "}
            {node.folder.parentId > -1 && editFolder(node.folder)}
          </div>
        </div>
        <ul style={{ listStyle: "none", paddingInlineStart: "25px" }}>
          {node.children && node.children.length
            ? node.children.map((child: IFolder) =>
                renderChildren({
                  folder: {
                    id: child.id,
                    parentId: child.parentId,
                    name: child.name,
                  },
                  children: foldersStore.foldersList.filter(
                    (x) => x.parentId === child.id
                  ),
                })
              )
            : null}
        </ul>
      </li>
    );
  };

  return (
    <ul className={styles.ulRoot} style={{}} role="tree">
      {renderChildren({
        folder: {
          ...foldersStore.rootFolder,
        },
        children: foldersStore.foldersList.filter((x) => x.parentId === 0),
      })}
    </ul>
  );
});
