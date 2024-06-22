import React, { useState, useEffect, useMemo } from 'react';
import { LineChart } from '@mui/x-charts';
import { Box, Slider, Grid } from '@mui/material';
import { ButtonContainer, DateButtonGroup, StyledButton } from './RouteGraph.styles'; // Импорт стилей
import { getSurfaceColor, getSpeedColor, formatUnixTime } from './RouteGraph.utils'; // Импорт вспомогательных функций
import DownloadExcelButton from '../DownloadExcelButton';
import UploadFileButton from '../UploadFileButton';

/**
 * Компонент для отображения графика маршрута с возможностью загрузки данных и интерактивным управлением.
 */
const RouteGraph = () => {
    const [data, setData] = useState(null); // Состояние для хранения данных
    const [xAxisRange, setXAxisRange] = useState([0, 1000]); // Диапазон значений оси X
    const [selectedRoute, setSelectedRoute] = useState(null); // Выбранный маршрут

    // Загрузка данных с сервера при монтировании компонента
    const fetchData = () => {
        fetch('https://localhost:5000/api/routeData/PointsAndTracks') // Путь к вашему API
            .then((response) => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then((json) => {
                console.log("Data fetched:", json);
                setData(json);
                const firstRoute = Object.keys(json)[0];
                setSelectedRoute(firstRoute);
            })
            .catch((error) => {
                console.error("Error fetching data:", error);
            });
    };

    // Загрузка данных с сервера при монтировании компонента
    useEffect(() => {
        fetchData();
    }, []); // Пустой массив зависимостей для выполнения эффекта один раз при монтировании

    // Обработчик загрузки файла на сервер
    const handleFileUpload = (file) => {
        const formData = new FormData();
        formData.append('file', file);

        fetch('https://localhost:5000/api/FileUpload/upload', { // Путь к API методу загрузки файла
            method: 'POST',
            body: formData,
        })
            .then(response => response.json())
            .then(jsonResponse => {
                console.log('JSON response from server:', jsonResponse);
                fetchData(); // Заново вызываем fetchData для получения обновленных данных
            })
            .catch(error => {
                console.error('Ошибка при загрузке файла на сервер:', error);
            });
    };

    // Мемоизированные вычисления данных для графика
    const { xAxisData, lineData, lineColors, areaData, areaColors, maxXValue, maxYValue } = useMemo(() => {
        if (!data || !selectedRoute || !data[selectedRoute]) {
            console.log("На выбранном маршруте не найдено точек или маршрутов");
            return {
                xAxisData: [],
                lineData: [],
                lineColors: [],
                areaData: [],
                areaColors: [],
                maxXValue: 0,
                maxYValue: 0
            };
        }

        const { points, tracks } = data[selectedRoute];

        if (!points || !tracks) {
            console.log("Не удалось найти точки или маршруты на выбранном маршруте");
            return {
                xAxisData: [],
                lineData: [],
                lineColors: [],
                areaData: [],
                areaColors: [],
                maxXValue: 0,
                maxYValue: 0
            };
        }

        const xAxisData = [];
        const lineData = [];
        const lineColors = [];
        const areaData = [];
        const areaColors = [];

        let cumulativeDistance = 0;

        tracks.forEach((track) => {
            const point1 = points.find((point) => point.id === track.firstId);
            const point2 = points.find((point) => point.id === track.secondId);

            if (!point1 || !point2) return;

            const firstPoint = { x: cumulativeDistance, y: point1.height };
            const secondPoint = { x: cumulativeDistance + track.distance, y: point2.height };

            xAxisData.push(firstPoint.x);
            lineData.push(firstPoint.y);
            lineColors.push(getSpeedColor(track.maxSpeed));

            xAxisData.push(secondPoint.x);
            lineData.push(secondPoint.y);
            lineColors.push(getSpeedColor(track.maxSpeed));

            areaData.push(firstPoint.y, secondPoint.y);
            areaColors.push(getSurfaceColor(track.surface), getSurfaceColor(track.surface));

            cumulativeDistance += track.distance;
        });

        return {
            xAxisData,
            lineData,
            lineColors,
            areaData,
            areaColors,
            maxXValue: cumulativeDistance,
            maxYValue: Math.max(...points.map(p => p.height))
        };
    }, [data, selectedRoute]); // Зависимости для мемоизации данных

    // Установка диапазона оси X при изменении максимального значения
    useEffect(() => {
        if (maxXValue > 0) {
            setXAxisRange([0, maxXValue]);
        }
    }, [maxXValue]);

    // Вывод заглушки при загрузке данных
    if (!data) return <div>Loading...</div>;

    // Отрисовка компонента с графиком и элементами управления
    return (
        <Grid container spacing={2}>
            <Grid item xs={12}>
                <ButtonContainer>
                    <DownloadExcelButton />
                    <UploadFileButton onFileUpload={handleFileUpload} />
                </ButtonContainer>
                <DateButtonGroup variant="contained">
                    {Object.keys(data).map(timestamp => (
                        <StyledButton
                            key={timestamp}
                            onClick={() => setSelectedRoute(timestamp)}
                            variant={selectedRoute === timestamp ? 'contained' : 'outlined'}
                        >
                            {formatUnixTime(timestamp)}
                        </StyledButton>
                    ))}
                </DateButtonGroup>
            </Grid>
            <Grid item xs={12}>
                <Box sx={{ position: 'relative', height: 400, width: '80%', mx: 'auto' }}>
                    <LineChart
                        xAxis={[
                            {
                                id: 'line',
                                data: xAxisData,
                                label: "Пройденное расстояние",
                                min: xAxisRange[0],
                                max: xAxisRange[1],
                                colorMap: {
                                    type: 'piecewise',
                                    thresholds: xAxisData,
                                    colors: lineColors,
                                },
                            },
                            {
                                id: "area",
                                min: xAxisRange[0],
                                max: xAxisRange[1],
                                data: xAxisData,
                                colorMap: {
                                    type: "piecewise",
                                    thresholds: xAxisData,
                                    colors: areaColors,
                                },
                            },
                        ]}
                        yAxis={[
                            {
                                min: 0,
                                max: maxYValue
                            }
                        ]}
                        series={[
                            {
                                data: lineData,
                                xAxisKey: 'line',
                                label: "Высота точки",
                            },
                            {
                                data: new Array(xAxisData.length).fill(maxYValue),
                                xAxisKey: "area",
                                area: true,
                                showMark: false,
                                color: 'transparent',
                                backgroundColor: 'rgba(0, 0, 0, 0)',
                                areaStyle: { opacity: 0.1 },
                                legend: { display: false },
                            },
                        ]}
                        tooltip={{
                            trigger: 'item',
                        }}
                        grid={{ vertical: true, horizontal: true }}
                        axisHighlight={{ x: 'none', y: 'none' }}
                    />
                </Box>
            </Grid>
            <Grid item xs={12}>
                <Box sx={{ width: '80%', mx: 'auto' }}>
                    <Slider
                        value={xAxisRange}
                        onChange={(event, newValue) => setXAxisRange(newValue)}
                        min={0}
                        max={maxXValue}
                        valueLabelDisplay="auto"
                        aria-labelledby="range-slider"
                    />
                </Box>
            </Grid>
        </Grid>
    );
};

export default RouteGraph;
