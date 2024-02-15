import { Component, AfterViewInit, NgZone, OnInit,Input } from '@angular/core';
import { LocalizationService } from '../../core';

@Component({
    selector: 'bcare-slider',
    templateUrl: './slider.component.html',
    styleUrls: ['./slider.component.css']
})
export class SliderComponent implements OnInit {
  isEnglish;
  @Input() isWafierRequest : boolean;
  @Input() isRenualRequest: boolean;
  @Input() referenceId: string;
  @Input() isCustomCard: boolean;
  ngOnInit(): void { 
 $('.hideInSecondStep').css('display', 'block');
}
    constructor(private zone: NgZone, private _localizationService: LocalizationService) {
        //debugger;
      this.isEnglish = _localizationService.getCurrentLanguage().id === 2;
 $('.hideInSecondStep').css('display', 'block');
    }
}


// import { Component, OnInit } from '@angular/core';
// import * as $ from 'jquery';
// import 'slick-carousel';
// @Component({
//   selector: 'bcare-slider',
//   templateUrl: './slider.component.html',
//   styleUrls: ['./slider.component.css']
// })
// export class SliderComponent implements OnInit {

//   constructor() { }

//   ngOnInit() {
//     $('#top-slider, .top-slider').slick({
//       dots: true,
//       rtl: true,
//       slidesToShow: 1,
//       slidesToScroll: 1,
//       arrows: false,
//       autoplay: true,
//       autoplaySpeed: 5000,
//       asNavFor: '.slides .slider',
//       appendDots: $('.arrows-move')
//     });
//     $('.slides .slider').slick({
//       dots: false,
//       rtl: true,
//       asNavFor: '#top-slider, .top-slider',
//       arrows: false,
//       slidesToShow: 1,
//       slidesToScroll: 1,
//       autoplay: true,
//       autoplaySpeed: 5000
//     });
//   }

// }
