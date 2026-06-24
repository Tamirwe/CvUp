import { Box } from "@mui/material";
import { useEffect, useState } from "react";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";
import { CandsSourceEnum } from "../models/GeneralEnums";
import { ICand, ISearchModel } from "../models/GeneralModels";
import { sortCandList } from "../utils/GeneralUtils";
import { SearchControl } from "../components/header/SearchControl";
import { CandsList } from "../components/cands/CandsList";

export const PositionGroupCandsList = observer(() => {
  const { candsStore, positionsStore } = useStore();
  const [list, setList] = useState<ICand[]>([]);
  const [positionsTypesAdvancedOpen, setPositionsTypesAdvancedOpen] = useState(false);

  useEffect(() => {
    setList(candsStore.posTypeCandsList);
  }, [candsStore.posTypeCandsList]);

  const sortList = (isDesc: boolean) => {
    setList(sortCandList(isDesc, candsStore.posTypeCandsList));
  };

  const handleSearch = (searchVals: ISearchModel) => {
    if (searchVals.value) {
      candsStore.searchPositionTypeCands(searchVals);
    } else if (positionsStore.selectedPositionType?.id) {
      candsStore.getPositionTypeCandsList(positionsStore.selectedPositionType.id);
    }
  };

  return (
    <>
      <Box mt={1} mr={1} ml={1} sx={{ overflow: "hidden" }}>
        <SearchControl
          onSearch={handleSearch}
          onShowAdvanced={() =>
            setPositionsTypesAdvancedOpen(!positionsTypesAdvancedOpen)
          }
          shoeAdvancedIcon={true}
          records={candsStore.posTypeCandsList && candsStore.posTypeCandsList.length}
          showSort={true}
          onSort={sortList}
          showRefreshList={true}
        />
      </Box>
      <CandsList
        candsListData={list}
        candsSource={CandsSourceEnum.PositionType}
        advancedOpen={positionsTypesAdvancedOpen}

      />
    </>
  );
});
