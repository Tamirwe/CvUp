import { Box } from "@mui/material";
import { useState } from "react";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";
import { CandsSourceEnum, TabsCandsEnum } from "../models/GeneralEnums";
import { ICand, ISearchModel } from "../models/GeneralModels";
import { SearchControl } from "../components/header/SearchControl";
import { CandsList } from "../components/cands/CandsList";

interface IProps {
  candsFolderList: ICand[];
  onSearch: (searchVals: ISearchModel) => void;
  onSort: (isDesc: boolean) => void;
}

export const FolderCandsList = observer(
  ({ candsFolderList, onSearch, onSort }: IProps) => {
    const { candsStore } = useStore();
    const [foldersAdvancedOpen, setFoldersAdvancedOpen] = useState(false);

    return (
      <>
        <Box mt={1} mr={1} ml={1}>
          <SearchControl
            onSearch={onSearch}
            onShowAdvanced={() => setFoldersAdvancedOpen(!foldersAdvancedOpen)}
            shoeAdvancedIcon={true}
            records={
              candsStore.folderCandsList && candsStore.folderCandsList.length
            }
            showSort={true}
            onSort={onSort}
          />
        </Box>
        <CandsList
          candsListData={candsFolderList}
          candsSource={CandsSourceEnum.Folder}
          advancedOpen={foldersAdvancedOpen}
        />
      </>
    );
  },
);
