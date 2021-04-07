export default interface DataItem
{
    id: string;
    type: "text" | "list" | "number";
    value: any;
}