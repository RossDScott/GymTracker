import { Atom, PrimitiveAtom, useAtom } from "jotai";
import { ExerciseSetVM, SetWeightMetrics } from "../../../shared/session.model";

export type props = {
    setAtom: PrimitiveAtom<ExerciseSetVM<SetWeightMetrics>>;
}

const WeightSet = (props: props) => {
    const [set, setSet] = useAtom(props.setAtom);

    const handleCompletedChange = () => {
        setSet({...set, completed: !set.completed});
    }

    const completedCircleClass = set.completed ? 'bi-check-circle' : 'bi-circle';
    return (
        <div className="row d-flex align-items-center align-self-center">
            <div className="col fw-bold">{set.setType}</div>
            <div className="col-2 text-center p-0">{set.targetMetrics.weight}</div>
            <div className="col-1 text-center p-0 me-3">{set.targetMetrics.reps}</div> 
            <div className="col-2 text-center p-0">{set.actualMetrics?.weight}</div>
            <div className="col-1 text-center p-0 me-3">{set.actualMetrics?.reps}</div>
            <div className="col-1 text-center">
                <i className={`bi bi-check-circle ${completedCircleClass}`} onClick={handleCompletedChange}></i>
            </div>
        </div>
    )
}

export default WeightSet;