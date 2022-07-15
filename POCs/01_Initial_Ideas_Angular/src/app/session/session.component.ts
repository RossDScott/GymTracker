import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-session',
    templateUrl: './session.component.html',
    styleUrls: ['./session.component.css']
})
export class SessionComponent implements OnInit {

    constructor() { }

    editRow = false;
    rowConfirmed = false;

    ngOnInit(): void {
    }

    handleEditRow(){
        this.editRow = true;
    }

    handleConfirm(event: Event){
        event.cancelBubble = true;
        this.rowConfirmed = true;
    }

    handleConfimEdit(){
        this.editRow = false;
    }

}
