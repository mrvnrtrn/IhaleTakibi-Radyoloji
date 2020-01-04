import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../entity/service/authentication.service';

@Component({
  selector: 'app-top-menu',
  templateUrl: './top-menu.component.html',
  styleUrls: ['./top-menu.component.css']
})
export class TopMenuComponent implements OnInit {
  isLogin : boolean = false;
  constructor(private authenticationService: AuthenticationService) { }

  ngOnInit() {
    if (this.authenticationService.currentUserValue) {
      this.isLogin = true;
    }
    else
    {
      this.isLogin = false;
    }
  }  
}
