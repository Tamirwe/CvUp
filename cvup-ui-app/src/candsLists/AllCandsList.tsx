import { Box } from "@mui/material";
import { useEffect, useState } from "react";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";
import { CandsSourceEnum } from "../models/GeneralEnums";
import { ICand, ISearchModel } from "../models/GeneralModels";
import { sortCandList } from "../utils/GeneralUtils";
import { SearchControl } from "../components/header/SearchControl";
import { CandsList } from "../components/cands/CandsList";

export const AllCandsList = observer(() => {
  const { candsStore } = useStore();
  const [list, setList] = useState<ICand[]>([]);
  const [candsAdvancedOpen, setCandsAdvancedOpen] = useState(false);
  const [isAISelected, setIsAISelected] = useState(false);

  useEffect(() => {
    setList(candsStore.allCandsList);
  }, [candsStore.allCandsList]);

  const sortList = (isDesc: boolean) => {
    setList(sortCandList(isDesc, candsStore.allCandsList));
  };

  const handleAI = (selected: boolean, searchValue: string) => {
    setIsAISelected(selected);
    if (selected) {
      candsStore.AiSearchCands({ value: searchValue, exact: false });
    }
  };

  const handleSearch = (searchVals: ISearchModel) => {
    if (isAISelected) {
      candsStore.AiSearchCands(searchVals);
    } else if (searchVals.value) {
      candsStore.searchAllCands(searchVals);
    } else {
      candsStore.getCandsList();
    }
  };

  return (
    <>
      <Box mt={1} mr={1} ml={1}>
        <SearchControl
          onSearch={handleSearch}
          onShowAdvanced={() => setCandsAdvancedOpen(!candsAdvancedOpen)}
          shoeAdvancedIcon={false}
          records={candsStore.allCandsList && candsStore.allCandsList.length}
          showSort={true}
          onSort={sortList}
          showRefreshList={true}
          extSearch={candsStore.extSearch}
          showAI={false}
          onAI={handleAI}
          showSE={true}
        />
      </Box>
      <CandsList
        candsListData={isAISelected ? candsStore.aiCandsResults : list}
        candsSource={CandsSourceEnum.AllCands}
        advancedOpen={candsAdvancedOpen}

      />
    </>
  );
});
