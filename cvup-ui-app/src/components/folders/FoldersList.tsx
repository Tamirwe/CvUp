import { IconButton, List } from "@mui/material";
import { observer } from "mobx-react-lite";
import { MouseEventHandler } from "react";
import { CiEdit } from "react-icons/ci";
import { useStore } from "../../Hooks/useStore";
import { CrudTypesEnum } from "../../models/GeneralEnums";
import { IFolder, IFolderNode } from "../../models/GeneralModels";
import styles from "./FoldersList.module.scss";

export const FoldersList = observer(() => {
  const { foldersStore, generalStore } = useStore();
  // let vv = 1;

  const editFolder = (folder: IFolder) => {
    return (
      <IconButton
        size="small"
        onClick={async () => {
          foldersStore.selectedFolder = folder;
          generalStore.openModeFolderFormDialog = CrudTypesEnum.Update;
          // if (vv === 1) {
          //   const conf = await generalStore.confirmDialog("gfhgf", "fghfgh")();

          //   console.log(conf);
          // } else {
          //   const conf = await (
          //     await generalStore.confirmDialog(
          //       "gfsdfsdfsdfsdfsdfsdfhgf",
          //       "sdfsdfsdfsdfsdfsdfsdfsdfsdfsdf"
          //     )
          //   )();

          //   console.log(conf);
          // }

          // vv = 2;
        }}
      >
        <CiEdit />
      </IconButton>
    );
  };

  const renderChildren = (node: IFolderNode) => {
    return (
      <li
        key={node.folder.id}
        role="treeitem"
        style={{ cursor: "pointer", lineHeight: 1 }}
      >
        <div className={styles.folderLine} onClick={(event) => {}}>
          <div>{node.folder.name}</div>
          <div className={styles.editFolder}>
            {" "}
            {node.folder.parentId > -1 && editFolder(node.folder)}
          </div>
        </div>
        <ul style={{ listStyle: "none", paddingInlineStart: "25px" }}>
          {Array.isArray(node.children)
            ? node.children.map((node: IFolder) =>
                renderChildren({
                  folder: {
                    id: node.id,
                    parentId: node.parentId,
                    name: node.name,
                  },
                  children: foldersStore.foldersList.filter(
                    (x) => x.parentId === node.id
                  ),
                })
              )
            : null}
        </ul>
      </li>
    );
  };

  return (
    <ul
      style={{
        position: "relative",
        display: "block",
        listStyle: "none",
        paddingInlineStart: "10px",
        marginTop: "5px",
      }}
      role="tree"
    >
      {renderChildren({
        folder: {
          id: 0,
          parentId: -1,
          name: "Folders",
        },
        children: foldersStore.foldersList,
      })}
    </ul>
  );
});
