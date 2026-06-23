import { Box } from "@mui/material";
import { useState } from "react";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";
import { CandsSourceEnum, TabsCandsEnum } from "../models/GeneralEnums";
import { ICand, ISearchModel } from "../models/GeneralModels";
import { SearchControl } from "../components/header/SearchControl";
import { CandsList } from "../components/cands/CandsList";

interface IProps {
  allCandsList: ICand[];
  onSearch: (searchVals: ISearchModel) => void;
  onSort: (isDesc: boolean) => void;
}

export const AllCandsList = observer(
  ({ allCandsList, onSearch, onSort }: IProps) => {
    const { candsStore } = useStore();
    const [candsAdvancedOpen, setCandsAdvancedOpen] = useState(false);

    return (
      <>
        <Box mt={1} mr={1} ml={1}>
          <SearchControl
            onSearch={onSearch}
            onShowAdvanced={() => setCandsAdvancedOpen(!candsAdvancedOpen)}
            shoeAdvancedIcon={true}
            records={candsStore.allCandsList && candsStore.allCandsList.length}
            showSort={true}
            onSort={onSort}
            showRefreshList={true}
            extSearch={candsStore.extSearch}
          />
        </Box>
        <CandsList
          candsListData={allCandsList}
          candsSource={CandsSourceEnum.AllCands}
          advancedOpen={candsAdvancedOpen}
          showAiDetails={true}
        />
      </>
    );
  },
);
