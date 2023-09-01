import { Fab, IconButton, Slide } from "@mui/material";
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
import {
  MdKeyboardArrowUp,
  MdPersonAddAlt1,
  MdPersonRemove,
} from "react-icons/md";
import { useCallback, useEffect, useRef, useState } from "react";
import { isMobile } from "react-device-detect";
import classNames from "classnames";

export const FoldersList = observer(() => {
  const { foldersStore, candsStore, generalStore } = useStore();
  const listRef = useRef<any>(null);
  const [rootFoldersList, setRootFoldersList] = useState<IFolder[]>([]);
  const [isScrolled, setIsScrolled] = useState(false);

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
    const sTop = instance.scrollTop;

    if (sTop > 50) {
      setIsScrolled(true);
    } else {
      setIsScrolled(false);
    }

    if (instance.scrollHeight - instance.clientHeight < sTop + 150) {
      if (rootFoldersList) {
        const numRecords = rootFoldersList.length;
        const newPosList = rootFoldersList.concat(
          foldersStore.sortedFolders?.slice(numRecords, numRecords + 50)
        );
        setRootFoldersList(newPosList);
      }
    }
  }, [rootFoldersList]);

  useEffect(() => {
    const instance = listRef.current;

    instance.addEventListener("scroll", onScroll);

    return () => {
      instance.removeEventListener("scroll", onScroll);
    };
  }, [onScroll]);

  const handleEditFolderClick = (folder: IFolder) => {
    if (isMobile) {
      generalStore.leftDrawerOpen = false;
    }

    foldersStore.editFolderSelected = folder;
    generalStore.openModeFolderFormDialog = CrudTypesEnum.Update;
  };

  const handleDeatchClick = async (
    event: React.MouseEvent<HTMLButtonElement | HTMLAnchorElement>,
    folder: IFolder
  ) => {
    event.stopPropagation();
    event.preventDefault();

    if (isMobile) {
      generalStore.leftDrawerOpen = false;
    }

    await foldersStore.detachCandidate(folder.id);
  };

  const handleAttachClick = async (
    event: React.MouseEvent<HTMLButtonElement | HTMLAnchorElement>,
    folder: IFolder
  ) => {
    event.stopPropagation();
    event.preventDefault();

    if (isMobile) {
      generalStore.leftDrawerOpen = false;
    }

    await foldersStore.attachCandidate(folder.id);
  };

  const editFolder = (folder: IFolder) => {
    return (
      <div className={styles.iconsDiv}>
        <IconButton
          size="small"
          sx={{ color: "#dfdfdf" }}
          onClick={() => handleEditFolderClick(folder)}
        >
          <CiEdit />
        </IconButton>
        {candsStore.candDisplay &&
        candsStore.candDisplay?.candFoldersIds.indexOf(folder.id) > -1 ? (
          <IconButton
            size="small"
            sx={{ color: "#ff8d00" }}
            onClick={(event) => handleDeatchClick(event, folder)}
          >
            <MdPersonRemove />
          </IconButton>
        ) : (
          <IconButton
            size="small"
            sx={{ color: "#dfdfdf" }}
            onClick={(event) => handleAttachClick(event, folder)}
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
    <>
      <ul
        className={classNames({
          [styles.ulRoot]: true,
          [styles.isMobile]: isMobile,
        })}
        role="tree"
        ref={listRef}
      >
        {renderChildren({
          folder: {
            ...foldersStore.rootFolder,
          },
          children: rootFoldersList.filter((x) => x.parentId === 0),
        })}
      </ul>
      <Slide direction="up" in={isScrolled} mountOnEnter unmountOnExit>
        <Fab
          size="medium"
          color="primary"
          sx={{
            zIndex: 999,
            position: "fixed",
            bottom: "7rem",
            left: "4rem",
            fontSize: "2rem",
          }}
          onClick={() => {
            listRef.current.scrollTop = 0;
          }}
        >
          <MdKeyboardArrowUp />
        </Fab>
      </Slide>
    </>
  );
});
