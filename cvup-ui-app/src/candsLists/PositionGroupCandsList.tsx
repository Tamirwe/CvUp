import { Box } from "@mui/material";
import { useState } from "react";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";
import { CandsSourceEnum, TabsCandsEnum } from "../models/GeneralEnums";
import { ICand, ISearchModel } from "../models/GeneralModels";
import { SearchControl } from "../components/header/SearchControl";
import { CandsList } from "../components/cands/CandsList";

interface IProps {
  candsPosTypeList: ICand[];
  onSearch: (searchVals: ISearchModel) => void;
  onSort: (isDesc: boolean) => void;
}

export const PositionGroupCandsList = observer(
  ({ candsPosTypeList, onSearch, onSort }: IProps) => {
    const { candsStore } = useStore();
    const [positionsTypesAdvancedOpen, setPositionsTypesAdvancedOpen] =
      useState(false);

    return (
      <>
        <Box mt={1} mr={1} ml={1} sx={{ overflow: "hidden" }}>
          <SearchControl
            onSearch={onSearch}
            onShowAdvanced={() =>
              setPositionsTypesAdvancedOpen(!positionsTypesAdvancedOpen)
            }
            shoeAdvancedIcon={true}
            records={
              candsStore.posTypeCandsList && candsStore.posTypeCandsList.length
            }
            showSort={true}
            onSort={onSort}
            showRefreshList={true}
          />
        </Box>
        <CandsList
          candsListData={candsPosTypeList}
          candsSource={CandsSourceEnum.PositionType}
          advancedOpen={positionsTypesAdvancedOpen}
        />
      </>
    );
  },
);
