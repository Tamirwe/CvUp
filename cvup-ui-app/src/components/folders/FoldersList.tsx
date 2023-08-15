import { IconButton } from "@mui/material";
import { observer } from "mobx-react-lite";
import { CiEdit } from "react-icons/ci";
import { useStore } from "../../Hooks/useStore";
import {
  CrudTypesEnum,
  TabsCandsEnum,
  TabsGeneralEnum,
} from "../../models/GeneralEnums";
import { IFolder, IFolderNode } from "../../models/GeneralModels";
import styles from "./FoldersList.module.scss";
import { MdPersonAddAlt1, MdPersonRemove } from "react-icons/md";
import { useCallback, useEffect, useRef, useState } from "react";
import { isMobile } from "react-device-detect";

export const FoldersList = observer(() => {
  const { foldersStore, candsStore, generalStore } = useStore();
  const listRef = useRef<any>(null);
  const [rootFoldersList, setRootFoldersList] = useState<IFolder[]>([]);

  useEffect(() => {
    if (!foldersStore.foldersList.length) {
      foldersStore.getFoldersList();
    }
  }, []);

  useEffect(() => {
    if (generalStore.currentLeftDrawerTab === TabsGeneralEnum.Folders) {
      if (candsStore.candDisplay?.candFoldersIds.length) {
        foldersStore.displayCandFolders();
      }
    }
  }, [candsStore.candDisplay, generalStore.currentLeftDrawerTab]); // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    //if (foldersStore.sortedFolders.length > 0) {
    const numRecords = rootFoldersList.length;
    setRootFoldersList(foldersStore.sortedFolders?.slice(0, numRecords + 50));
    //}
  }, [foldersStore.sortedFolders]); // eslint-disable-line react-hooks/exhaustive-deps

  const onScroll = useCallback(() => {
    const instance = listRef.current;

    if (
      instance.scrollHeight - instance.clientHeight <
      instance.scrollTop + 150
    ) {
      if (rootFoldersList) {
        const numRecords = rootFoldersList.length;
        const newPosList = rootFoldersList.concat(
          foldersStore.sortedFolders?.slice(numRecords, numRecords + 50)
        );
        setRootFoldersList(newPosList);
      }

      console.log(instance.scrollTop);
    }
  }, [rootFoldersList]);

  useEffect(() => {
    const instance = listRef.current;

    instance.addEventListener("scroll", onScroll);

    return () => {
      instance.removeEventListener("scroll", onScroll);
    };
  }, [onScroll]);

  const editFolder = (folder: IFolder) => {
    return (
      <div className={styles.iconsDiv}>
        <IconButton
          size="small"
          sx={{ color: "#dfdfdf" }}
          onClick={async (event) => {
            // event.stopPropagation();
            // event.preventDefault();

            if (isMobile) {
              generalStore.leftDrawerOpen = false;
            }

            foldersStore.editFolderSelected = folder;
            generalStore.openModeFolderFormDialog = CrudTypesEnum.Update;
          }}
        >
          <CiEdit />
        </IconButton>
        {candsStore.candDisplay &&
        candsStore.candDisplay?.candFoldersIds.indexOf(folder.id) > -1 ? (
          <IconButton
            size="small"
            sx={{ color: "#ff8d00" }}
            onClick={async (event) => {
              event.stopPropagation();
              event.preventDefault();

              if (isMobile) {
                generalStore.leftDrawerOpen = false;
              }

              await foldersStore.detachCandidate(folder.id);
            }}
          >
            <MdPersonRemove />
          </IconButton>
        ) : (
          <IconButton
            size="small"
            sx={{ color: "#dfdfdf" }}
            onClick={async (event) => {
              event.stopPropagation();
              event.preventDefault();

              if (isMobile) {
                generalStore.leftDrawerOpen = false;
              }

              await foldersStore.attachCandidate(folder.id);
            }}
          >
            <MdPersonAddAlt1 />
          </IconButton>
        )}
      </div>
    );
  };

  const renderChildren = (node: IFolderNode) => {
    return (
      <li
        key={node.folder.id}
        role="treeitem"
        style={{
          cursor: "pointer",
          lineHeight: 1,
          backgroundColor:
            node.folder.id === foldersStore.selectedFolder?.id
              ? "ButtonFace"
              : "unset",
        }}
      >
        <div
          className={styles.folderLine}
          onClick={() => {
            if (
              candsStore.currentTabCandsLists !== TabsCandsEnum.FolderCands ||
              foldersStore.selectedFolder?.id !== node.folder.id
            ) {
              if (isMobile) {
                generalStore.leftDrawerOpen = false;
                generalStore.rightDrawerOpen = true;
              }

              foldersStore.selectedFolder = node.folder;
              candsStore.getFolderCandsList();
              candsStore.currentTabCandsLists = TabsCandsEnum.FolderCands;
            }
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
                  children: rootFoldersList.filter(
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
    <ul className={styles.ulRoot} style={{}} role="tree" ref={listRef}>
      {renderChildren({
        folder: {
          ...foldersStore.rootFolder,
        },
        children: rootFoldersList.filter((x) => x.parentId === 0),
      })}
    </ul>
  );
});
