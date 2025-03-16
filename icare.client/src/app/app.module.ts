import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { AboutComponent } from './about/about.component';
import { AdminComponent } from './admin/admin.component';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ChangePasswordComponent } from './auth/change-password/change-password.component';
import { LoginComponent } from './auth/login/login.component';
import { RecoverPasswordComponent } from './auth/recover-password/recover-password.component';
import { RegisterComponent } from './auth/register/register.component';
import { ResetPasswordComponent } from './auth/reset-password/reset-password.component';
import { HeaderComponent } from './header/header.component';
import { HomeComponent } from './home/home.component';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { ProfileComponent } from './profile/profile.component';
import { RecipesComponent } from './recipes/recipes.component';
import { RecipeComponent } from './recipe/recipe.component';
import { InventoryComponent } from './inventory/inventory.component';
import { MapKeysPipe } from './pipes/map-keys-pipe';
import { GoalsComponent } from './goals/goals.component';
import { CalendarComponent } from './calendar/calendar.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';

import { ContainerComponent } from './components/container/container.component';
import { ProfileImageComponent } from './components/profile-image/profile-image.component';
import { CustomInputComponent } from './components/custom-input/custom-input.component';
import { CustomCheckboxComponent } from './components/custom-checkbox/custom-checkbox.component';
import { CategorySelectorComponent } from './components/category-selector/category-selector.component';
import { SearchBarComponent } from './components/search-bar/search-bar.component';
import { HelpComponent } from './components/help/help.component';
import { GoalComponent } from './goal/goal.component';

@NgModule({
  declarations: [
    AppComponent,
    AboutComponent,
    ProfileComponent,
    HomeComponent,
    NavMenuComponent,
    LoginComponent,
    RegisterComponent,
    RecoverPasswordComponent,
    ResetPasswordComponent,
    AdminComponent,
    HeaderComponent,
    ChangePasswordComponent,
    RecipesComponent,
    RecipeComponent,
    InventoryComponent,
    MapKeysPipe,
    GoalsComponent,
    CalendarComponent,
    ContainerComponent,
    ProfileImageComponent,
    CustomInputComponent,
    CustomCheckboxComponent,
    CategorySelectorComponent,
    SearchBarComponent,
    HelpComponent,
    GoalComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserModule,
    BrowserAnimationsModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    MatNativeDateModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
  ],
  bootstrap: [AppComponent, NavMenuComponent],
  exports: [
    MapKeysPipe
  ]
})
export class AppModule {}
