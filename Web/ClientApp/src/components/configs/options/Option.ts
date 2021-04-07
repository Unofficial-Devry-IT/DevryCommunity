export class BaseOption
{
    name: string = "";
}

/**
 * Used when the option requires a true/false value
 */
export class BoolOption extends BaseOption
{
    value: boolean = false;
}

/**
 * Used when the option requires a number to be selected
 */
export class NumberOption extends BaseOption
{
    value: number = 0;
}

/**
 * Used when the option requires a single role to be selected
 */
export class RoleOption extends BaseOption
{
    value: string = ""; // ulong cannot be stored as integers in JavaScript
}

/**
 * Used when the option requires multiple roles to be selected
 */
export class RolesOption extends BaseOption
{
    value: string[] = [];
}

/**
 * Used when the option shall only be text
 */
export class TextOption extends BaseOption
{
    value: string = "";
}

/**
 * Used when the option requires a number range
 */
export class NumberRangeOption extends BaseOption
{
    min: number = 0;
    max: number = 100;
}

export default {
    NumberRangeOption,
    NumberOption,
    BoolOption,
    RolesOption,
    RoleOption
}