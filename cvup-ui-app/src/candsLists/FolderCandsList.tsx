import { Box } from "@mui/material";
import { useEffect, useState } from "react";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";
import { CandsSourceEnum } from "../models/GeneralEnums";
import { ICand, ISearchModel } from "../models/GeneralModels";
import { sortCandList } from "../utils/GeneralUtils";
import { SearchControl } from "../components/header/SearchControl";
import { CandsList } from "../components/cands/CandsList";

export const FolderCandsList = observer(() => {
  const { candsStore } = useStore();
  const [list, setList] = useState<ICand[]>([]);
  const [foldersAdvancedOpen, setFoldersAdvancedOpen] = useState(false);

  useEffect(() => {
    setList(candsStore.folderCandsList);
  }, [candsStore.folderCandsList]);

  const sortList = (isDesc: boolean) => {
    setList(sortCandList(isDesc, candsStore.folderCandsList));
  };

  const handleSearch = (searchVals: ISearchModel) => {
    if (searchVals.value) {
      candsStore.searchFolderCands(searchVals);
    } else {
      candsStore.getFolderCandsList();
    }
  };

  return (
    <>
      <Box mt={1} mr={1} ml={1}>
        <SearchControl
          onSearch={handleSearch}
          onShowAdvanced={() => setFoldersAdvancedOpen(!foldersAdvancedOpen)}
          shoeAdvancedIcon={true}
          records={candsStore.folderCandsList && candsStore.folderCandsList.length}
          showSort={true}
          onSort={sortList}
        />
      </Box>
      <CandsList
        candsListData={list}
        candsSource={CandsSourceEnum.Folder}
        advancedOpen={foldersAdvancedOpen}
      />
    </>
  );
});
