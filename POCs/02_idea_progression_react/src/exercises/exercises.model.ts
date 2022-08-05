export type MetricType = "weight" | "time";

export interface Exercise{
    id: number;
    metricType: MetricType;
    name: string;
}

export interface SetType{
    id: number;
    name: string;
}