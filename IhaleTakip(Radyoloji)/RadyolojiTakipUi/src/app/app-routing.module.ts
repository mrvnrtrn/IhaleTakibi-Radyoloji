import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RadyolojiKayitComponent } from '../app/radyoloji-modulu/radyoloji-kayit/radyoloji-kayit.component';
import { AuthGuard } from './entity/service/auth.guard';
import { LoginComponent } from './login';
import { LogoutComponent } from './logout/logout.component';


const routes: Routes = [    
  { path: '', component: RadyolojiKayitComponent,canActivate: [ AuthGuard ] },
  { path: 'radyoloji-islem-kayit', component: RadyolojiKayitComponent,canActivate: [ AuthGuard ] },
  {path:'giris', component:LoginComponent},
  {path:'logout', component:LogoutComponent, canActivate: [ AuthGuard ] },
];

@NgModule({
  imports: [RouterModule.forRoot(routes,{onSameUrlNavigation:'reload'})],
  exports: [RouterModule]
})
export class AppRoutingModule { }
