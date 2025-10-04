import { useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { observer } from "mobx-react";
import styles from "./ListOHLC.module.scss";
import { Iohlc } from "../../models/FuStatModel";

interface IProps {
  onEdit: (ohlcRow: Iohlc) => void;
}

export const ListOHLC = observer(({ onEdit }: IProps) => {
  const { futuresStatisticStore } = useStore();

  const handleEditRow = (ohlcRow: Iohlc) => {
    onEdit(ohlcRow);
  };

  const handleDeleteRow = (id: number) => {
    futuresStatisticStore.deleteDayOhlc(id);
  };

  return (
    <div className={styles.wrapper}>
      <div className={styles.tableWrapper}>
        <table>
          <thead>
            <tr>
              <th>Date</th>
              <th>OPEN</th>
              <th>HIGH</th>
              <th>LOW</th>
              <th>CLOSE</th>
              <th>Points</th>
            </tr>
          </thead>
          <tbody>
            {futuresStatisticStore.dailyStatList?.map((item, i) => {
              return (
                <TableRow
                  item={item}
                  onEdit={handleEditRow}
                  onDelete={handleDeleteRow}
                  key={item.id}
                />
              );
            })}
          </tbody>
        </table>
      </div>
    </div>
  );
});

interface ITableRowProps {
  item: Iohlc;
  onEdit: (ohlcRow: Iohlc) => void;
  onDelete: (id: number) => void;
}

const TableRow = ({ item, onEdit, onDelete }: ITableRowProps) => {
  const [isHovered, setIsHovered] = useState(false);

  return (
    <tr
      onMouseEnter={() => setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
    >
      <td>
        {item.statisticDate &&
          new Date(item.statisticDate).toLocaleDateString("en-US", {
            year: "numeric",
            day: "2-digit",
            month: "short",
          })}
      </td>
      <td>{item.open}</td>
      <td>{item.high}</td>
      <td>{item.low}</td>
      <td>{item.close}</td>
      <td>
        {item.dayPoints}
        {isHovered && (
          <div className={styles.deleteButtonWrapper}>
            <div>
              <button data-color={"edit"} onClick={() => onEdit(item)}>
                Edit
              </button>
              <button
                data-color={"delete"}
                onClick={() => onDelete(item.id || 0)}
              >
                Delete
              </button>
            </div>
          </div>
        )}
      </td>
    </tr>
  );
};
