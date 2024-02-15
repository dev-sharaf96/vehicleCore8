import { Component, OnInit } from '@angular/core';
import { PolicyService } from '../core/services/policy.service';
import { Bank } from '../core';

@Component({
  selector: 'app-banks',
  templateUrl: './banks.component.html',
  styleUrls: ['./banks.component.css']
})
export class BanksComponent implements OnInit {
  banks: Bank[];
  constructor(private _policyService: PolicyService) { }

  ngOnInit() {
    this._policyService.getAllBanks().subscribe(banks => this.banks = banks);
  }
  changeActivity(code) {
    console.log(code);
  }
}
