import { Chart } from "react-google-charts";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { Iohlc } from "../../models/FuStatModel";

interface IProps {}

export const ChartCandlestick = observer(({}: IProps) => {
  const { futuresStatisticStore } = useStore();
  const [chartDataAndTitle, setChartDataAndTitle] = useState<any[]>([]);

  let ohlcDataList: Iohlc[] = [];

  useEffect(() => {
    futuresStatisticStore.dailyStatList && loadChartData();
  }, [futuresStatisticStore.dailyStatList]);

  const loadChartData = () => {
    const chartTitle = [["Day", "LOW", "OPEN", "CLOSE", "HIGH"]];
    const reversedArray = [...futuresStatisticStore.dailyStatList].reverse();

    const chartData = reversedArray.map((item, i) => {
      return [
        new Date(item.statisticDate!).toLocaleDateString("en-US", {
          year: "2-digit",
          month: "2-digit",
          day: "2-digit",
        }),
        item.low,
        item.open,
        item.close,
        item.high,
      ];
    });

    setChartDataAndTitle([...chartTitle, ...chartData]);
    console.log(chartDataAndTitle);
  };

  const data = [
    ["Day", "", "", "", ""],
    ["Mon", 20, 28, 38, 45],
    ["Tue", 31, 38, 55, 66],
    ["Wed", 50, 55, 77, 80],
    ["Thu", 77, 77, 66, 50],
    ["Fri", 68, 66, 22, 15],
  ];

  const options = {
    legend: "none",
    bar: { groupWidth: "100%" }, // Remove space between bars.
    candlestick: {
      fallingColor: { strokeWidth: 0, fill: "#a52714" }, // red
      risingColor: { strokeWidth: 0, fill: "#0f9d58" }, // green
    },
  };

  return (
    <div>
      <Chart
        chartType="CandlestickChart"
        width="700px"
        height="700px"
        data={chartDataAndTitle}
        options={options}
      />
    </div>
  );
});
