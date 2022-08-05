import { useCallback } from "react";
import { atom, useAtom, useAtomValue } from "jotai";
import { useUpdateAtom } from "jotai/utils";
import { exercisesAtom as availableExercisesAtom } from "../../../exercises/exercises.atoms";
import { exercisesAtom } from "../../shared/session.atoms";
import { ExerciseVM, SetMetrics } from "../../shared/session.model";


const showAddAtom = atom(false);

const AddExercise = () => {
    const [showAdd, setShowAdd] = useAtom(showAddAtom);
    const setExercise = useUpdateAtom(exercisesAtom);
    const availableExercises = useAtomValue(availableExercisesAtom)

    const handleAddExercise = (exerciseId: number) => {
        const exercise = availableExercises.find(x => x.id === exerciseId)!;
        const newEx = {completed: false, sets: [], exercise: {name: exercise.name, metricType: exercise.metricType, targetSets: []}} as ExerciseVM<SetMetrics>;
        setExercise(exercises => [...exercises, newEx]);
        setShowAdd(false);
    }

    const handleShowAddExercise = () => {
        setShowAdd(true);
    }

    const addButton = (
        <li className="list-group-item">
            <i onClick={() => setShowAdd(true)} className="bi bi-plus-circle-fill fs-3"></i>
        </li>
    )

    const showListRef = useCallback((node: any) => {
        if (node !== null) {
            node.focus();
        }
      }, []);

    const addList = (
        <div className="input-group">
            <select ref={showListRef} className="form-select me-2" onChange={(e) => handleAddExercise(+e.target.value)} onBlur={() => setShowAdd(false)} >
                <option value={0}>Choose...</option>
                {availableExercises.map(exercise => <option key={exercise.id} value={exercise.id}>{exercise.name}</option>)}
            </select>
        </div>
    );

    return showAdd ? addList : addButton;
}

export default AddExercise;