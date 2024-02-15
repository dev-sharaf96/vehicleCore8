import { Component, OnInit,Input } from '@angular/core';

@Component({
  selector: 'bcare-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  constructor() { }
    @Input() isWafierRequest : boolean;
    @Input() isRenualRequest: boolean;
    @Input() referenceId: string;
    @Input() isCustomCard: boolean;
     ngOnInit() {
  }

}
